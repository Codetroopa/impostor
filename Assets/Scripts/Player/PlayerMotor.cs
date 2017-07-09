﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMotor : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Vector3 velocity;

    PlayerController con;

	// Use this for initialization
	void Start() {
        con = GetComponent<PlayerController>();
	}
	
	void Update() {
        Vector3 prevPos = transform.position;

        // Check direction and return if none has been pressed
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Calculate movement speeds
        float speed = calculateSpeed();

        velocity.x = direction.x * speed;
        velocity.y = direction.y * speed;
        
        // Rotate and Move player accordingly
        if (con.gunDrawn) {
            Vector3 lookPos = GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;
            con.Rotate(lookPos);
        } else {
            con.Rotate(velocity * Time.deltaTime);
        }
        con.Move(velocity * Time.deltaTime);
    }

    float calculateSpeed() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return runSpeed;
        }

        return walkSpeed;
    }
}
