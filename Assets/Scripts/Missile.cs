using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    public float rotationSpeed = 10f; // How fast it snaps to its direction

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionRadius = 3.5f;

    private Vector3 moveDirection;
    private bool hasExploded = false;

    public void Launch(Transform player)
    {
        // Set initial direction
        moveDirection = (player.position - transform.position).normalized;
    }

    void Update()
    {
        // 1. Move the missile
        transform.position += moveDirection * speed * Time.deltaTime;

        // 2. Rotate the missile to face where it is going
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            // Calculate the angle from the movement vector
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            /* IMPORTANT: Since your object points UP naturally, 
               we subtract 90 degrees so the "Top" faces the "Right" (0 degrees).
            */
            Quaternion targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            // Smoothly rotate toward that direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded || collision.isTrigger || collision.CompareTag("Enemy")) return;

        Explode();
    }

    public void Explode()
    {
        hasExploded = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Multiplayer Damage Logic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var pc = hit.GetComponent<PlayerController>();
                if (pc != null) pc.DieAndRespawn();
            }
        }

        Destroy(gameObject);
    }
}