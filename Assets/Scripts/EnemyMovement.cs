using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState
{
    moveleft = 0,
    running = 1,
    air = 2,
    slash = 3
}



public class EnemyMovement : MonoBehaviour
{

    Rigidbody2D rb;
    Collider2D box;
    //Animator animator;
    state lastState;

    float speed;
    int dir = 1;
    GameObject CollidedObjectH;
    GameObject CollidedObjectV;
    public string wallTag;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        box = gameObject.GetComponent<Collider2D>();
        //animator = gameObject.GetComponent<Animator>();
        speed = 5;
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 v = rb.velocity;
        Vector2 pos = rb.transform.position;

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
            RaycastHit2D collided;

            if (Physics2D.Raycast(boxPointsLeft[i], new Vector2(-1, 0), 0.05f))
            {
                hit.left = true;
                collided = Physics2D.Raycast(boxPointsLeft[i], new Vector2(-1, 0), 0.05f);
                CollidedObjectH = collided.collider.gameObject;
            }
            if (Physics2D.Raycast(boxPointsRight[i], new Vector2(1, 0), 0.05f))
            {
                hit.right = true;
                collided = Physics2D.Raycast(boxPointsRight[i], new Vector2(1, 0), 0.05f);
                CollidedObjectH = collided.collider.gameObject;
            }
            if (Physics2D.Raycast(boxPointsTop[i], new Vector2(0, 1), 0.05f))
            {
                hit.top = true;
                collided = Physics2D.Raycast(boxPointsTop[i], new Vector2(0, 1), 0.05f);
                CollidedObjectV = collided.collider.gameObject;
            }
            if (Physics2D.Raycast(boxPointsBottom[i], new Vector2(0, -1), 0.05f))
            {
                hit.bottom = true;
                collided = Physics2D.Raycast(boxPointsBottom[i], new Vector2(0, -1), 0.05f);
                CollidedObjectV = collided.collider.gameObject;
            }

           

            Debug.DrawRay(boxPointsLeft[i], new Vector2(-0.05f, 0));
            Debug.DrawRay(boxPointsRight[i], new Vector2(0.05f, 0));
            Debug.DrawRay(boxPointsTop[i], new Vector2(0, 0.05f));
            Debug.DrawRay(boxPointsBottom[i], new Vector2(0, -0.05f));
        }


        //v.x = speed * dir;



        //RaycastHit2D hitRight = Physics2D.Raycast(rightPos, new Vector2(1, 0), speed * Time.deltaTime, 5);
        //RaycastHit2D hitLeft = Physics2D.Raycast(leftPos, new Vector2(-1, 0), speed * Time.deltaTime, 5);

        //Debug.DrawRay(rightPos, new Vector2(speed * Time.deltaTime, 0));
        //Debug.DrawRay(leftPos, new Vector2(-speed * Time.deltaTime, 0));

        if (hit.left && CollidedObjectH.tag == wallTag)
        {
            if (v.x < 0)
            {
                dir *= -1;
            }
        }
        if (hit.right && CollidedObjectH.tag == wallTag)
        {
            if (v.x > 0)
            {
                dir *= -1; ;
            }
        }
        if (hit.bottom && CollidedObjectV.tag == wallTag)
        {
            //v.y = 0;
        }

        v.x = speed * dir;
        //{
        //hit2D.collider.gameObject.active = false;


        // If so, stop the movement
        //v.x = 0;
        //}

        if (currentState != lastState)
        {
            lastState = currentState;


            switch (currentState)
            {
                case state.idle:

                    break;
                case state.running:

                    break;
                case state.air:

                    break;
                case state.slash:

                    break;
            }
            //animator.Play(anim);
        }

        rb.velocity = v;
    }
}
