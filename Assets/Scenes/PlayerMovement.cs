using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int move_accel;
    public int max_move_speed;
    public int jump_velocity;
    public bool isRightHeld;
    public bool isLeftHeld;
    public bool isGrounded;
    private Rigidbody2D rb;


    // Debugging variables
    public Vector2 initialPos;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        max_move_speed = 5;
        move_accel = 10;
        jump_velocity = 7;
        isGrounded = false;

        initialPos = transform.position;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if the float value of veritcal velocity is aproximate to zero,
        // the player is probably grounded right? 
        if (Math.Abs(rb.velocity.y) < 0.00001)
        {
            isGrounded = true;
        }
    
    }

    // Update is called once per frame
    void Update () {
        // setting temp variables because assigning stuff straight to the
        // class variables (especially Rigidbody2D), can be finnicky sometimes
        Vector2 v = rb.velocity;
        Vector2 p = transform.position;

        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;
        
        int accelX = 0;

        // movement is constant when grounded and acceleration based when in air
        if (isGrounded) {
            // the horizontal velocity only resets if it is less than the max speed,
            // this allows the player to slide after propulsion
            // the player can immediately break this slide by moving in the opposite direction
            // if they so choose
            // just an idea, however, it feels a bit weird atm with how it just stops after sliding
            if (Math.Abs(v.x) < max_move_speed) {
                v.x = 0;
            }

            // if the player is grounded, there movement velocity is constant
            // we should test to see if this is the right feel
            // I believe this needs fine tuning
            if (Input.GetKey(KeyCode.D))
            {
                
                if (v.x < max_move_speed)
                {
                    v.x = max_move_speed;
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (v.x > -max_move_speed)
                {
                    v.x = -max_move_speed;
                }
            }

            // if player is grounded, they can jump
            if (Input.GetButtonDown("Jump"))
            {
                v.y = jump_velocity;
                isGrounded = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (v.x < max_move_speed)
                {
                    accelX = move_accel;
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (v.x > -max_move_speed)
                {
                    accelX = -move_accel;
                }
            }

            // increment velocity based on acceleration and time passed
            float gainedVelocity = ((float)accelX) * Time.deltaTime;
            v.x += gainedVelocity;
        }

        // upon clicking, the player propels themself in the opposite direction of the click
        if (Input.GetMouseButtonDown(0)) {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));

            double angle = Math.Atan2(clickPos.y - p.y, clickPos.x - p.x);
            float volX = -(float)(Math.Cos(angle) * 10);
            float volY = -(float)(Math.Sin(angle) * 10);
            v.x = volX;
            v.y = volY;
        }

        // returning velocity result to gameObject
        rb.velocity = v;

        // setting isGrounded to false just incase player falls off a platform
        isGrounded = false;

        // reset game if player falls too low
        if (p.y < -10) {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
