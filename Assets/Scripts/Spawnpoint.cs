using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public int level;
    public bool unlocked;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward * 1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        unlocked = true;
    }
}