using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 40f;
    
    public void TakeDamage(float damage){
        
        health -= damage;
        
        if(health <= 0){
            Die();
            health = 0; // Optional.
        }
    }
    
    void Die(){
        Destroy(gameObject);
    }
    
}
