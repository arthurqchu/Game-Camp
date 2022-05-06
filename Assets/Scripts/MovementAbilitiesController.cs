using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Invector.vCharacterController;
public class MovementAbilitiesController : MonoBehaviour
{
    public Transform playerGrapplePoint;
    public Transform playerTransform;

    public float airJumpForce = 50f;
    public int numAirJumps=1;

    public float thrustModifier = 1f;
    public float maxThrustSpeed = 5f;

    public float springiness = 4.5f;
    public float damper = 7f;
    public float maxDistance = 100f;
    public float massScale = 10f;

    public bool canThrust;

    public float wallGrabDistance = 1f;

    public Material lineMaterial;
    public float ropeWidth;




    [SerializeField] InputActionAsset input;
    [SerializeField] Camera cam;

    private Animator animator;
    private Rigidbody rb;
    private bool thrust = false;
    private Vector3 mouseWorldPosition;
    private SpringJoint joint;
    private GameObject grappleSphere;
    private ArticulationBody articulation;
    private LineRenderer lr;
    private Vector2 Move;

    private vThirdPersonController controller;

    private bool pulling = false;
    private bool jumped = false;
    private bool grounded;
    private int tempJumps;
    
    private void Awake() {
        //When this gameobject wakes up/loads, we get the components we need. Using GetComponent<>(), unity looks at the other components and grabs the one that matches
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        articulation = GetComponent<ArticulationBody>();
        controller = GetComponent<vThirdPersonController>();
    }

    private void Update() {
        bool grounded = animator.GetBool("IsGrounded");
        Move = input.actionMaps[0].FindAction("Move").ReadValue<Vector2>();


        //This part gets the mouse's world position and sets the z to zero
        Vector2 mousePosition = input.actionMaps[0].FindAction("Aim").ReadValue<Vector2>();
        mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,-cam.transform.position.z));
        mouseWorldPosition.z = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y,0);

        Collider[] touching = Physics.OverlapSphere(playerTransform.position + new Vector3(0,1,0),0.5f);
        foreach (var hitCollider in touching)
        {
            if(hitCollider.tag == "Environment")
            {
                
                if(Vector3.Distance(hitCollider.ClosestPoint(playerTransform.position), playerTransform.position) < wallGrabDistance)
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionY;
                    Debug.Log("Touching");
                }else {
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                }
            }
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;


        // rb.constraints = RigidbodyConstraints.None;

        //Reset jumps
        if(grounded&&tempJumps<=0)
        {
            tempJumps = numAirJumps;
        }
       

        //Thrust if holding spacebar if canThrust is true. We apply physics in FixedUpdate() because that one is constant. Update() is erratic and would apply the forces weird
        if(canThrust)
        {
            if(input.actionMaps[0].FindAction("Space").ReadValue<float>()==1f)
            {
                thrust = true;
            }else {
                thrust = false;
            }
        }
        

        //Springjoint on right click
        if(joint==null&&input.actionMaps[0].FindAction("RightClick").ReadValue<float>()==1f)
        {
            controller.SetGrounding(false);
            //Debug.Log("grapple!");

            //Makes a sphere at the mouseWorldPosition that we set earlier ^^^
            grappleSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            grappleSphere.transform.position = mouseWorldPosition;


            


            //Creates a SpringJoint at our sphere position
            joint = gameObject.AddComponent<SpringJoint>();

            joint.autoConfigureConnectedAnchor = false;//Tell it that we want to set it manually
            joint.anchor = Vector3.zero; //This is (0,0,0) relative to the character's position, so the center of the character
            joint.connectedAnchor = mouseWorldPosition;//The SpringJoint goes from our player to the grappleSphere
            joint.maxDistance = maxDistance;//This is the distance away from the player that the spring won't apply springy. It's weird I know
            joint.spring = springiness;//Set the spring's springiness from the public variable way at the top
            joint.damper = damper;//How unspringy is the sprint I think. I'm still experimenting with different values
            joint.massScale = massScale;//This will change the mass of the player in terms of the math for doing the forces of the spring behind the scene. Bigger number makes player lighter
            // lr.SetPosition(0, joint.transform.position);  //This is supposed to set one end of the line. still working on it
            // lr.SetPosition(1, joint.connectedAnchor);

            Collider[] hitColliders = Physics.OverlapSphere(mouseWorldPosition, 1f);
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.tag == "Pullable")
                {
                    pulling = true;
                    joint.connectedBody = hitCollider.gameObject.GetComponent<Rigidbody>();
                    joint.connectedAnchor = Vector3.zero;
                    Destroy(grappleSphere);
                }
            }
        }
        // if(joint!=null)
        // {
        //     joint.connectedAnchor = mouseWorldPosition;
        //     grappleSphere.transform.position = mouseWorldPosition;
        // }
        if(joint!=null&&input.actionMaps[0].FindAction("RightClick").ReadValue<float>()==0f)// If there's a SpringJoint but we are not holding down the right mouse button
        {
            //Debug.Log("NO grapple!");
            controller.SetGrounding(true);

            pulling = false;
            Destroy(joint);//Self-explanatory
            Destroy(grappleSphere);//We also destroy the sphere because we're just using it as a way to see the other end of the spring

        }
    }
    private void LateUpdate() { //I don't really know why but it looks weird if the line is drawn in Update(). LateUpdate()runs after Update()
        if(joint!=null)
        {
            lr.enabled = true;

            lr.material = lineMaterial;
            lr.SetWidth(ropeWidth,ropeWidth);

            lr.SetPosition(0, playerTransform.position + Vector3.up);
            if(pulling)
            {
                lr.SetPosition(1,joint.connectedBody.position);
            }else{
                lr.SetPosition(1, joint.connectedAnchor);
            }
 
        }
        if(joint == null)
        {
            lr.enabled = false;
        }
            
    }

    private void FixedUpdate() {//Always apply continuous physics in FixedUpdate! You want the physics to be steady
        if(thrust&& rb.velocity.magnitude<maxThrustSpeed)
        {
            rb.AddForce(thrustModifier * -Physics.gravity * rb.mass);
        }
    }

    public void OnJump()
    {

        //Double Jump
        if(!grounded&&tempJumps>0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            rb.AddForce(airJumpForce * Vector3.up,ForceMode.Impulse);

            jumped = true;
            tempJumps-=1;
            Debug.Log("Jump!");
            
        }
        // if(grounded&&jumped)
        // {
        //     numAirJumps+=1;
        //     jumped = false;
        // }

    }
}
