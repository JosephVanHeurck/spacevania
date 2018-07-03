using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayer : MonoBehaviour
{

    RaycastHit2D[] Find;
    RaycastHit2D Clear;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 FindPlayerCoords()
    {
        Find = Physics2D.CircleCastAll(this.transform.position, 10, Vector2.zero);
        Vector2 PlayerCoords;

        int i = 0;
        while (i < Find.Length)
        {
            if (Find[i].collider.gameObject.name != "player")
            {
                i++;
            }
            else if (Find[i].collider.gameObject.name == "player")
            {
                PlayerCoords = Find[i].collider.gameObject.transform.position;
                return PlayerCoords;
            }
        }


        return Vector2.zero;
    }

    public bool PathToPlayerClear(Vector2 pCoords)
    {
        Clear = Physics2D.Linecast(this.transform.position, pCoords);
        if (Clear)
        {
            if (Clear.collider.gameObject.name == "player")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //and if the line cast hits nothing? I gues its clear??
        return true;
    }
}
