﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim3d : MonoBehaviour
{
    public LayerMask layerMask;
    
    Vector3 mousePos;
    Vector3 lookDir;
    
    void Update()
    {
        
        RaycastHit camHit;
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Vector3 direction = new Vector3();
        
        if (Physics.Raycast(camRay, out camHit)) {
            Transform objectHit = camHit.transform;
            direction = transform.position - camHit.point;
        }
        
         RaycastHit hit;
        if (Physics.Raycast(transform.position, -direction, out hit, Mathf.Infinity, layerMask))
        {
            direction.y = 0;
            Debug.DrawRay(transform.position, -direction * 5, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.white);
            Debug.Log("Did not Hit");
        }
        
        //---Rotation---
        mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        lookDir.x = mousePos.x - objectPos.x;
        lookDir.y = mousePos.y - objectPos.y;
        
        //From FixedUpdate update.
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 90-angle, 0));
        
    }
}