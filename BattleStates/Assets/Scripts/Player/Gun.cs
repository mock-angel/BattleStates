using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 30f;
    
    public GameObject player;
    public ParticleSystem muzzleFlash;
    
    private float nextTimeToFire = 0f;
    
    void Update(){
        
        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire){
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }
    
    void Shoot(){
        muzzleFlash.Play();
        
        RaycastHit hit;
        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, range)){
            Debug.Log(hit.transform.name); // Optional.
            
            Target target = hit.transform.GetComponent<Target>();
            if(target != null){
                target.TakeDamage(damage);
            }
            
        }
    }
}
