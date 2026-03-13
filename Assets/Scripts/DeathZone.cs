using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RespawnPlayer(collision.gameObject);
        }
    }

    void RespawnPlayer(GameObject player)
    {
        player.transform.position = new Vector3(0, 0, 0);
    }
}