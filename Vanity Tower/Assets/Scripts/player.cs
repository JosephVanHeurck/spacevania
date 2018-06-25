using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class sidesHit {
    public bool left;
    public bool right;
    public bool top;
    public bool bottom;

    public List<RaycastHit2D> rayCasts = new List<RaycastHit2D>();

    public sidesHit() {
        left = false;
        right = false;
        top = false;
        bottom = false;
    }
}

public class adjustedPropulsion {
    public String playerXDirection = "none";
    public float newXPropulsion;
    public bool hasMovedOpposite = false;

    public void isOppositePropulsion() {
        hasMovedOpposite = true;
    }

    public void reset() {
        playerXDirection = "none";
        newXPropulsion = 0;
        hasMovedOpposite = false;
    }
}


enum state {
    idle = 0,
    running = 1,
    air = 2,
    slash = 3
}


public class player : MonoBehaviour {
    
    Rigidbody2D rb;
    Collider2D box;
    Animator animator;
    state lastState;

    float speed;
    float jumpSpeed;

    Vector3 propulsionVelocity;
    adjustedPropulsion adjustedProp = new adjustedPropulsion();

    float attackTime;

    bool isGrounded;


	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        box = gameObject.GetComponent<Collider2D>();
        animator = gameObject.GetComponent<Animator>();
        speed = 5;
        propulsionVelocity = new Vector3(0,0,0);
        //attackTime = 0.2f;
        jumpSpeed = 8;
        isGrounded = false;
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //isTouchingObject = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        //isTouchingObject = false;
    }

    // Update is called once per frame
    void Update () {
        Vector2 v = rb.velocity;
        Vector3 pv = propulsionVelocity;
        Vector2 pos = rb.transform.position;
        adjustedPropulsion adjusted = adjustedProp;

        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;

        Vector2 size = box.bounds.size;

        state currentState = lastState;

        Vector2[] boxPointsLeft = new Vector2[] {
            new Vector2(pos.x - size.x / 2, pos.y + size.y / 2),
            new Vector2(pos.x - size.x / 2, pos.y),
            new Vector2(pos.x - size.x / 2, pos.y - size.y / 2)
        };

        Vector2[] boxPointsRight = new Vector2[] {
            new Vector2(pos.x + size.x / 2, pos.y + size.y / 2),
            new Vector2(pos.x + size.x / 2, pos.y),
            new Vector2(pos.x + size.x / 2, pos.y - size.y / 2)
        };

        Vector2[] boxPointsTop = new Vector2[] {
            new Vector2(boxPointsLeft[0].x, boxPointsLeft[0].y),
            new Vector2(pos.x, pos.y + size.y / 2),
            new Vector2(boxPointsRight[0].x, boxPointsRight[0].y)
        };

        Vector2[] boxPointsBottom = new Vector2[] {
            new Vector2(boxPointsLeft[2].x, boxPointsLeft[2].y),
            new Vector2(pos.x, pos.y - size.y / 2),
            new Vector2(boxPointsRight[2].x, boxPointsRight[2].y)
        };

        //Vector2 rightPos = new Vector2(pos.x + box.bounds.size.x / 2, pos.y);
        //Vector2 leftPos = new Vector2(pos.x - box.bounds.size.x / 2, pos.y);

        // using raytracing outwardly 3 times for each side in order to detect sides collided from movement
        sidesHit hit = new sidesHit();

        for (int i = 0; i < 3; i++)
        {
            // temp variable that holds raycast to be copied if raycast returns an object
            RaycastHit2D temp;

            if (temp = Physics2D.Raycast(boxPointsLeft[i], new Vector2(-1, 0), 0.05f))
            {
                hit.rayCasts.Add(temp);
                hit.left = true;
            }
            if (temp = Physics2D.Raycast(boxPointsRight[i], new Vector2(1, 0), 0.05f))
            {
                hit.rayCasts.Add(temp);
                hit.right = true;
            }
            if (temp = Physics2D.Raycast(boxPointsTop[i], new Vector2(0, 1), 0.05f))
            {
                hit.rayCasts.Add(temp);
                hit.top = true;
            }
            if (temp = Physics2D.Raycast(boxPointsBottom[i], new Vector2(0, -1), 0.05f))
            {
                hit.rayCasts.Add(temp);
                hit.bottom = true;
            }

            Debug.DrawRay(boxPointsLeft[i], new Vector2(-0.05f, 0));
            Debug.DrawRay(boxPointsRight[i], new Vector2(0.05f, 0));
            Debug.DrawRay(boxPointsTop[i], new Vector2(0, 0.05f));
            Debug.DrawRay(boxPointsBottom[i], new Vector2(0, -0.05f));
        }


        v.x = 0;

        bool justSlashed = false;


        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slash")) {
            Debug.Log("Slashing");
        }*/

        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).nameHash);


        if (Input.GetKey(KeyCode.Mouse0)) {

            Vector2 clickPos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));

            double angle = Math.Atan2(clickPos.y - pos.y, clickPos.x - pos.x);
            float volX = -(float)(Math.Cos(angle) * 10);
            float volY = -(float)(Math.Sin(angle) * 10);



            v.y = volY;

            pv.x = volX;

            //currentState = state.slash;
            // preventing next if statement from actuating during current update
            //justSlashed = true;
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") && !justSlashed)
        {
            currentState = state.idle;

            if (Input.GetKey(KeyCode.D) && !hit.right)
            {
                rb.transform.localScale = new Vector3(-1, 1, 1);
                currentState = state.running;
                v.x = speed;

                adjusted.playerXDirection = "right";
            }
            else if (!Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) && !hit.left)
            {
                rb.transform.localScale = new Vector3(1, 1, 1);
                currentState = state.running;
                v.x = -speed;

                adjusted.playerXDirection = "left";
            }
            else {
                adjusted.playerXDirection = "none";
            }

            if (hit.bottom)
            {
                isGrounded = true;
            }
            else
            {
                currentState = state.air;
                isGrounded = false;
            }

            if (Input.GetKey(KeyCode.Space) && isGrounded && !hit.top)
            {
                isGrounded = false;
                v.y = jumpSpeed;
            }
        }


        //RaycastHit2D hitRight = Physics2D.Raycast(rightPos, new Vector2(1, 0), speed * Time.deltaTime, 5);
        //RaycastHit2D hitLeft = Physics2D.Raycast(leftPos, new Vector2(-1, 0), speed * Time.deltaTime, 5);

        //Debug.DrawRay(rightPos, new Vector2(speed * Time.deltaTime, 0));
        //Debug.DrawRay(leftPos, new Vector2(-speed * Time.deltaTime, 0));

        if (hit.left) {
            if (v.x < 0)
            {
                v.x = 0;
            }
            // stopping propulsion
            if (pv.x < 0)
            {
                pv.x = 0;
            }
        } 
        if (hit.right) {
            if (v.x > 0)
            {
                v.x = 0;
            }
            // stopping propulsion
            if (pv.x > 0)
            {
                pv.x = 0;
            }
        }
        if (hit.top) {
            
        }
        //{
            //hit2D.collider.gameObject.active = false;


            // If so, stop the movement
            //v.x = 0;
        //}


        float co = 40; // drag cooeficient
        float de = 5; // deceleration

        // reducing propulsion velocity
        if (pv.x > 0) {
            pv.x = pv.x - ((pv.x - (pv.x * co / 100)) * Time.deltaTime); // drag

            pv.x -= de * Time.deltaTime; // decelleration
            if (pv.x < 0) {
                pv.x = 0;
            }
        }
        else if (pv.x < 0) {
            pv.x = pv.x - ((pv.x - (pv.x * co / 100)) * Time.deltaTime);

            pv.x += de * Time.deltaTime;
            if (pv.x > 0)
            {
                pv.x = 0;
            }
        }

        // calculating adjusted propulsion based on player input during propulsion
        // this is required to prevent propulsion from continuing as normal after
        // the player moves in the opposite direction
        if (propulsionVelocity.x != 0) {
            adjusted.newXPropulsion = propulsionVelocity.x;

            if (adjusted.playerXDirection != "none")
            {
                if (adjusted.playerXDirection == "right" && propulsionVelocity.x < 0)
                {
                    adjusted.isOppositePropulsion();
                }
                else if (adjusted.playerXDirection == "left" && propulsionVelocity.x > 0)
                {
                    adjusted.isOppositePropulsion();
                }
            }

            if (adjusted.hasMovedOpposite)
            {
                if (pv.x < 0 && adjusted.playerXDirection != "right")
                {
                    adjusted.newXPropulsion += speed;
                }
                else if (pv.x > 0 && adjusted.playerXDirection != "left")
                {
                    adjusted.newXPropulsion -= speed;
                }
                // prevents propulsion velocity from continuing in the opposite direction upon
                // player letting go of directional input
                if (adjusted.playerXDirection == "none") {
                    if (adjusted.newXPropulsion > 0 && pv.x < 0) {
                        pv.x = 0;
                        adjusted.reset();
                    }
                    else if (adjusted.newXPropulsion < 0 && pv.x > 0) {
                        pv.x = 0;
                        adjusted.reset();
                    }
                }
            }

            
        }
        else {
            adjusted.reset();
        }


        rb.velocity = new Vector2(v.x + adjusted.newXPropulsion, v.y);

        propulsionVelocity = pv;
        adjustedProp = adjusted;


        if (pv.x == 0)
        {
            Debug.Log("Propulsion stopped");
        }

        //Debug.Log("propulsion is " + pv.x.ToString());



        if (currentState != lastState)
        {
            lastState = currentState;
            String anim = "";

            switch (currentState)
            {
                case state.idle:
                    anim = "Idle";
                    break;
                case state.running:
                    anim = "Running";
                    break;
                case state.air:
                    anim = "Air";
                    break;
                case state.slash:
                    anim = "Slash";
                    break;
            }
            animator.Play(anim);
        }
	}
}
