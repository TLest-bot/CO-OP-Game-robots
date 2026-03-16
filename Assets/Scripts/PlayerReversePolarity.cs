using UnityEngine;

public class PlayerReversePolarity : MonoBehaviour
{
    [field: Header("Ability Attributes")]
    [field: SerializeField]
    public float strength;
    public float duration;
    public float cooldown;
    public bool reverse;

    private Magnetic magnet;
    private float t = 0;
    private float cooldownTime = 0;
    private int normalPolarisation;
    private float normalStrength;
    void Awake()
    {
        if (gameObject.GetComponent<Magnetic>() != null)
        {
            magnet = gameObject.GetComponent<Magnetic>();
            normalPolarisation = magnet.polarity;
            normalStrength = magnet.strength;
        }
    }

    void OnPolarityAbility()
    {
        if (reverse && cooldownTime <= 0)
        {
            t = duration;
            cooldownTime = cooldown;
        }
    }

    void Update()
    {
        if (t >= 0)
        {
            t -= Time.deltaTime;
            UseAbility(normalPolarisation * -1, strength);
        }
        else
        {
            UseAbility(normalPolarisation, normalStrength);
        }

        if (cooldownTime >= 0)
        {
            cooldownTime -= Time.deltaTime;
        }
    }

    public void UseAbility(int polarity, float strength)
    {
        magnet.polarity = polarity;
        magnet.strength = strength;
    }
}
