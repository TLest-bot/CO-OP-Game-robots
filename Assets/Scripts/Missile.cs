using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    public float rotationSpeed = 10f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionRadius = 3.5f;
    public float explosionForce = 15f;

    private Vector3 moveDirection;
    private bool hasExploded = false;
    public AudioSource audioSource;

    public void Start()
    {
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    public void Launch(Transform player)
    {
        moveDirection = (player.position - transform.position).normalized;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded || collision.isTrigger || collision.CompareTag("Enemy")) return;

        hasExploded = true;

        GetComponent<Collider2D>().enabled = false;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Explode(transform.position, explosionRadius, explosionForce);

        Destroy(gameObject);
    }

    public void Explode(Vector2 explosionPos, float radius, float force)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController targetPlayer = hit.GetComponent<PlayerController>();
                if (targetPlayer == null) continue;

                Vector2 playerPos = hit.transform.position;
                Vector2 direction = (playerPos - explosionPos).normalized;
                float distance = Vector2.Distance(explosionPos, playerPos);
                Vector2 rayStart = explosionPos + (direction * 0.1f);
                int layerMask = LayerMask.GetMask("Walls", "Player");
                RaycastHit2D rayHit = Physics2D.Raycast(rayStart, direction, radius, layerMask);

                if (rayHit.collider != null && rayHit.collider.gameObject == hit.gameObject)
                {
                    float threshold = radius * 0.5f;

                    if (distance <= threshold && !targetPlayer.IsInputBlocked)
                    {
                        Debug.Log($"Killing {hit.name}");
                        targetPlayer.DieAndRespawn();
                    }
                    else
                    {
                        if (targetPlayer != null)
                        {
                            float forcePercent = 1f - ((distance - threshold) / threshold);
                            float finalForce = force * forcePercent;

                            targetPlayer.ReceiveExplosionForce(direction, finalForce);
                        }
                    }
                }
                if (hit.CompareTag("Enemy"))
                {
                    Vector2 enemyPos = hit.transform.position;
                    Vector2 dirToEnemy = (enemyPos - explosionPos).normalized;
                    Vector2 rayStartE = explosionPos + (dirToEnemy * 0.1f);

                    int enemyLayerMask = LayerMask.GetMask("Walls", "Enemy");
                    RaycastHit2D rayHitEnemy = Physics2D.Raycast(rayStartE, dirToEnemy, radius, enemyLayerMask);

                    if (rayHitEnemy.collider != null && rayHitEnemy.collider.gameObject == hit.gameObject)
                    {
                        Debug.Log($"Enemy {hit.name} destroyed by explosion!");
                        Destroy(hit.gameObject);
                    }
                }
            }
        }
    }
}
