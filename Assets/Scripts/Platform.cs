using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<GameObject> players;
    private BoxCollider2D myCol;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(myCol == null) { return; }
        foreach (GameObject player in players)
        {
            BoxCollider2D col = player.GetComponent<BoxCollider2D>();
            if(player.transform.position.y <= transform.position.y)
            {
                Physics2D.IgnoreCollision(myCol, col, true);
            }
            else
            {
                Physics2D.IgnoreCollision(myCol, col, false);
            }
        }
    }
}
