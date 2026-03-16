using UnityEngine;

public class WorldButton : MonoBehaviour
{
    [Header("Verbindingen")]
    public SequenceManager manager;

    [Header("Instellingen")]
    public int myID;
    public bool isReplayButton = false;

    public bool isPressed = false;
    private SpriteRenderer myRenderer;
    private AudioSource myAudio;

    void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<SequenceManager>();

        // AudioSource op DIT object (de trigger)
        myAudio = GetComponent<AudioSource>();

        // SpriteRenderer op de Parent (de button)
        myRenderer = GetComponentInParent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log is handig om te zien of de physics werkt
        if (other.CompareTag("Player") && !isPressed)
        {
            if (manager == null) return;

            if (isReplayButton)
            {
                // Start de hint-reeks
                manager.PlayTargetSequenceSounds();
                // Laat de replay knop zelf ook even oplichten
                StartCoroutine(manager.HandleReplayButtonFlash(this));
            }
            else
            {
                manager.OnButtonStepped(myID, this);
            }
        }
    }

    public void SetColor(Color c) { if (myRenderer != null) myRenderer.color = c; }
    public void PlaySound() { if (myAudio != null) myAudio.Play(); }
    public void SetPressState(bool state) { isPressed = state; }
}