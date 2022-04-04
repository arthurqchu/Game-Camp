using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float range = 20;
    public int health=1;

    public float explosionForce = 500f;
    public float explosionRadius = 1f;

    private NavMeshAgent nav;
    private BoxCollider box;

    /// <summary> Array that contains all children of given destructible object </summary>
    private MeshRenderer[] _destructibleObjects;



    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        _destructibleObjects = GetComponentsInChildren<MeshRenderer>();
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!box.enabled)
        {
            box.enabled = true;
        }
        Collider[] collisions = Physics.OverlapSphere(gameObject.transform.position, range);
        foreach(Collider hitCollider in collisions)
        {
            if(hitCollider.tag == "Player")
            {
                nav.SetDestination(hitCollider.transform.position);
            }
        }

        if(health <=0)
        {
            DestroyObj();
        }
    }

    public void Attack(int damage)
    {
        health-=damage;
    }

    // This function slices parent object into _destructibleObjects variable pieces
    public void DestroyObj() 
    {
        // Removes parental rigidbody and collider so we dont collide with them
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        // Now we go throught array of _destructibleObjects and make all children object independent!
        // add rigidbody and meshcollider on them so we get that effect of destructibliy
        foreach (MeshRenderer _dObj in _destructibleObjects)
        {
            // We need a default rigidbody for _destructibleObjects to have physics once separated from parental object 
            Rigidbody temp = _dObj.gameObject.AddComponent<Rigidbody>();

            // Mesh collider is best for smooth collision, but you can use other colliders
            _dObj.gameObject.AddComponent<MeshCollider>();
            _dObj.GetComponent<MeshCollider>().convex = true;

            // This makes sure _destructibleObjects become independent and move their own way
            _dObj.transform.SetParent(null);

            temp.AddExplosionForce(explosionForce, nav.transform.position, explosionRadius);
        }

        // This is here temporary to remove script from updating
        this.enabled = false;
    }
}
