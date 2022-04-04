using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int weaponDamage = 1;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Enemy")
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.Attack(weaponDamage);
            Debug.Log("Attack");

        }
    }
}
