using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    public float shieldThrowSpeed = 10f;
    public float shieldProjectileTime = 3f;
    [SerializeField] Animator animator;
    [SerializeField] GameObject shieldProjectile;
    [SerializeField] InputActionAsset input;
    [SerializeField] Camera cam;
    [SerializeField] GameObject equippedShield;

    private GameObject thrownShield;
    private Vector3 mouseWorldPosition;
    
    private void Awake() {
        animator = GetComponent<Animator>();
        cam = Camera.main;

    }

    private void Update() {
        //This part gets the mouse's world position and sets the z to zero
        Vector2 mousePosition = input.actionMaps[0].FindAction("Aim").ReadValue<Vector2>();
        mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,-cam.transform.position.z));
        mouseWorldPosition.z = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y,0);

        if(thrownShield != null)
        {
            equippedShield.SetActive(false);
        } else{
            equippedShield.SetActive(true);
        }
    }
    public void OnAttack()
    {
        Debug.Log("attack");
        animator.SetTrigger("Attack");
        
    }

    public void OnThrowShield()
    {
        Debug.Log("Throw Shield!");
        thrownShield = Instantiate(shieldProjectile,gameObject.transform.position+Vector3.up, Quaternion.identity);
        thrownShield.GetComponent<Rigidbody>().velocity = shieldThrowSpeed*Vector3.Normalize(mouseWorldPosition - gameObject.transform.position);
        Destroy(thrownShield,shieldProjectileTime);


    }
}
