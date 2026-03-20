using UnityEngine;

public class EnemyBoxAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 5f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;

    private Vector2 startPosition;
    private Transform playerTransform;
    private bool isChasing = false;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isChasing && playerTransform != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        float rightBound = startPosition.x + patrolDistance;
        float leftBound = startPosition.x - patrolDistance;

        if (transform.position.x >= rightBound) direction = -1;
        else if (transform.position.x <= leftBound) direction = 1;

        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }

    private void ChasePlayer()
    {
        float step = chaseSpeed * Time.deltaTime;
        Vector2 target = new Vector2(playerTransform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, step);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
        }
    }
}