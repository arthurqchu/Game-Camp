using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    public void OnAttack()
    {
        Debug.Log("attack");
        animator.SetTrigger("Attack");
        
    }
}
