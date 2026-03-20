using UnityEngine;
using System.Linq;

public class PlayerTeleportHandler : MonoBehaviour
{
    private SpawnPointManager globalManager;
    private float teleportCooldownTimer = 0f;
    public float TeleportCooldownDuration = 0.5f;

    void Start()
    {
        globalManager = Object.FindAnyObjectByType<SpawnPointManager>();
    }

    void Update()
    {
        if (teleportCooldownTimer > 0)
            teleportCooldownTimer -= Time.deltaTime;
    }

    public void PerformTeleport(Spawnpoint spawnpoint)
    {
        if (spawnpoint == null) return;

        CharacterController cc = GetComponent<CharacterController>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();


        if (cc != null) cc.enabled = false;

        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        transform.position = spawnpoint.transform.position;

        if (cc != null) cc.enabled = true;

        Debug.Log($"{gameObject.name} moved to {spawnpoint.level}");
    }

    private void TryManualTeleport(int level)
    {
        if (globalManager == null) return;

        Spawnpoint spawnpoint = globalManager.GetSpawnpointByLevel(level);

        if (spawnpoint != null && teleportCooldownTimer <= 0 && spawnpoint.unlocked)
        {
            PerformTeleport(spawnpoint);
            teleportCooldownTimer = TeleportCooldownDuration;
        }
    }

    void OnZeroButton() { TryManualTeleport(0); }
    void OnOneButton() { TryManualTeleport(1); }
    void OnTwoButton() { TryManualTeleport(2); }
    void OnThreeButton() { TryManualTeleport(3); }
    void OnFourButton() { TryManualTeleport(4); }
    void OnFiveButton() { TryManualTeleport(5); }
    void OnSixButton() { TryManualTeleport(6); }
    void OnSevenButton() { TryManualTeleport(7); }
    void OnEightButton() { TryManualTeleport(8); }
    void OnNineButton() { TryManualTeleport(9); }
}