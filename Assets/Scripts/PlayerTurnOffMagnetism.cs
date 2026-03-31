using UnityEngine;

public class PlayerTurnOffMagnetism : MonoBehaviour
{
    [field: Header("Ability Attributes")]
    [field: SerializeField]
    public float duration;
    public float cooldown;

    private Magnetic magnet;
    private float t = 0;
    private float cooldownTime = 0;
    private int normalPolarisation;
    private float normalStrength;

    public AudioSource MagneticSound;
    public bool isPlayer1;
    private bool a = false;
    void Awake()
    {

    }

    void OnPolarityAbility()
    {
        if (cooldownTime <= 0 && GetComponent<Magnetic>().activated)
        {
            MagneticSound.Play();
            t = duration;
            cooldownTime = cooldown;
        }
    }
    private void LateUpdate()
    {
        if(a == false)
        {
            a = true;
            if (gameObject.GetComponent<Magnetic>() != null)
            {
                magnet = gameObject.GetComponent<Magnetic>();
                normalPolarisation = magnet.polarity;
                normalStrength = magnet.strength;
            }
        }
    }


    void Update()
    {
        if (a)
        {
            if (t >= 0)
            {
                t -= Time.deltaTime;
                UseAbility(false);
            }
            else
            {
                UseAbility(true);
            }

            if (cooldownTime >= 0)
            {
                cooldownTime -= Time.deltaTime;
            }
        }
    }

    public void UseAbility(bool active)
    {
        GetComponent<PlayerController>().animator.SetBool("IsActivated", active);
        magnet.activated = active;
    }
}
