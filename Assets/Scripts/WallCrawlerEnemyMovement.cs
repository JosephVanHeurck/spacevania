using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum WallCrawlerMoveState
{
    climbing = 0,
    decending = 1,
    moveLeft = 2,
    moveRight = 3
}


public class WallCrawlerEnemyMovement : MonoBehaviour
{

    Rigidbody2D rb;
    Collider2D box;
    //Animator animator;
    //state lastState;

    float speed;
    int dir = 1;
    public bool Clockwise;
    GameObject CollidedObjectH;
    GameObject CollidedObjectV;
    
    public string wallTag;
    private WallCrawlerMoveState wallCrawlerMoveState;


    // Use this for initialization
    void Start()
    {
        wallCrawlerMoveState = WallCrawlerMoveState.moveRight;
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

        //state currentState = lastState;

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

        v.x = speed * dir;


        switch (wallCrawlerMoveState)
        {
            case WallCrawlerMoveState.moveRight:
                if (!hit.bottom)
                {
                    Debug.Log("climbing down");
                    wallCrawlerMoveState = WallCrawlerMoveState.decending;
                }

                if (hit.right && CollidedObjectH.tag == wallTag)
                {
                    Debug.Log("time to climb up");
                    wallCrawlerMoveState = WallCrawlerMoveState.climbing;
                }
                break;



            case WallCrawlerMoveState.moveLeft:
                if (!hit.top)
                {
                    //Debug.Log("top of the wall");
                    wallCrawlerMoveState = WallCrawlerMoveState.climbing;
                }

                if (hit.left && CollidedObjectH.tag == wallTag)
                {
                    //Debug.Log("top of the wall");
                    wallCrawlerMoveState = WallCrawlerMoveState.decending;
                    
                }
                break;



            case WallCrawlerMoveState.climbing:
                if (!hit.right)
                {
                    Debug.Log("top of the wall");
                    wallCrawlerMoveState = WallCrawlerMoveState.moveRight;
                }

                if (hit.top && CollidedObjectV.tag == wallTag)
                {
                    //Debug.Log("top of the wall");
                    wallCrawlerMoveState = WallCrawlerMoveState.moveLeft;
                }
                break;


            case WallCrawlerMoveState.decending:
                if (!hit.left)
                {
                    Debug.Log("bottom of the wall");
                    wallCrawlerMoveState = WallCrawlerMoveState.moveLeft;
                }

                if (hit.bottom && CollidedObjectV.tag == wallTag)
                {
                    Debug.Log("reached bottom, moving right");
                    wallCrawlerMoveState = WallCrawlerMoveState.moveRight;
                }
                break;
        }




        v.x = 0;
        v.y = 0;
        switch (wallCrawlerMoveState)
        {
            case WallCrawlerMoveState.moveRight:
                v.x = speed;
                break;

            case WallCrawlerMoveState.moveLeft:
                v.x = -speed;
                break;

            case WallCrawlerMoveState.climbing:
                v.y = speed;
                break;
            case WallCrawlerMoveState.decending:
                v.y = -speed;
                break;
        }

        rb.velocity = v;
        }
    }
