using System.Reflection;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    private float nextFireTime;
    private Transform targetPlayer;

    private void Update()
    {
        if (targetPlayer != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject go = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);

        Collider2D launcherCollider = GetComponent<Collider2D>();
        Collider2D missileCollider = go.GetComponent<Collider2D>();

        if (launcherCollider != null && missileCollider != null)
        {
            Physics2D.IgnoreCollision(launcherCollider, missileCollider);
        }

        go.GetComponent<Missile>().Launch(targetPlayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) targetPlayer = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) targetPlayer = null;
    }
}