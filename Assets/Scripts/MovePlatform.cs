using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [field: Header("Platform moving behaviour")]
    [field: SerializeField]
    public List<Transform> pointsToMoveThrough;
    public List<bool> stopAtPoints;
    public float speed;
    public float waitTime;
    public float startUpSpeed;
    public float slowDownSpeed;
    public float slowDownThreshold;
    public float minimumSpeed;

    [field: Header("State of platform")]
    [field: SerializeField]
    public bool isActivated;
    public bool pingPong = true;

    private float currentSpeed;
    private float t;
    private int currentPoint = 0;
    public int direction = 1; //1 is forwards, -1 is backwards
    public float dist;
    private Vector2[] vectorPoints;
    private Rigidbody2D rb;
    
    void Start()
    {
        vectorPoints = new Vector2[pointsToMoveThrough.Count];
        int i = 0;
        foreach (Transform pos in pointsToMoveThrough)
        {
            vectorPoints[i] = pos.position;
            i++;
        }

        currentSpeed = speed;
        t = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        dist = Vector2.Distance(transform.position, vectorPoints[currentPoint]);
        if (t >= 0)
        {
            t -= Time.deltaTime;
        }
        else
        {
            ManageDirection();
            if (isActivated)
            {
                Accelerate();
                MoveToPoint(currentPoint);
            }
        }     
    }

    public void MoveToPoint(int i)
    {
        /* float step = currentSpeed * Time.deltaTime; // calculate distance to move
         transform.position = Vector2.MoveTowards(transform.position, vectorPoints[i], step); */
        
        Vector2 dir = (vectorPoints[currentPoint - direction] - vectorPoints[currentPoint]).normalized * -1;
        Vector2 velocity = dir * currentSpeed;
        rb.linearVelocity = velocity;
    }

    public void ManageDirection()
    {
        Vector2 pos = transform.position;
        if (isActivated)
        {
            if (dist <= 0.1)
            {
                if (!pingPong)
                {
                    if (currentPoint == pointsToMoveThrough.Count - 1)
                    {
                        isActivated = false;
                    }
                }
                else
                {
                    if (currentPoint == pointsToMoveThrough.Count - 1 || (currentPoint == 0 && direction == -1))
                    {
                        t = waitTime;
                        currentSpeed = 0.01f;
                        direction *= -1;
                    }
                    else if (stopAtPoints[currentPoint])
                    {
                        t = waitTime;
                        currentSpeed = 0.01f;
                    }
                }
                currentPoint += direction;
            }
        }
    }

    public void Accelerate()
    {

        if(dist <= slowDownThreshold && stopAtPoints[currentPoint])
        {
            if (currentSpeed >= minimumSpeed)
            {
                currentSpeed -= Time.deltaTime * slowDownSpeed;
            }
        }
        else
        {
            if (currentSpeed <= speed)
            {
                currentSpeed += Time.deltaTime * startUpSpeed;
            }
        }
    }
}
