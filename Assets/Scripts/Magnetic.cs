using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    public List<BoxCollider2D> magneticColliders;

    [field: Header("Magnet attributes")]
    [field: SerializeField]
    public int polarity; //1 is postive polarity, -1 is negative polarity
    public bool isStatic;
    public float strength;
    public float range;

    public Rigidbody2D rb;

    void Awake()
    {
        GameObject.Find("Magnets Manager").GetComponent<MagnetsManager>().UpdateMagnetColliderList();
    }

    void Update()
    {
        foreach(BoxCollider2D col in magneticColliders)
        {
            if(rb != null)
            {
                if (AmIInRangeOfMagnet(col.gameObject) && !isStatic)
                {
                    GetPulledTowards(col);
                }
            }
        }
    }

    public Vector2 GetClosestMagneticPoint(BoxCollider2D col)
    {
        return col.ClosestPoint(transform.position);
    }


    //Debug
    private void OnDrawGizmosSelected()
    {
        foreach (BoxCollider2D col in magneticColliders)
        {
            Vector2 pos = GetClosestMagneticPoint(col);
            Gizmos.DrawWireSphere(pos, 0.4f);
        }

        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void GetPulledTowards(BoxCollider2D col)
    {
        Magnetic magnet = col.gameObject.GetComponent<Magnetic>();

        Vector2 myPos = transform.position;
        Vector2 dir = ((myPos - GetClosestMagneticPoint(col)).normalized*-1);
        float totalStrength = (magnet.strength*magnet.polarity) * (strength*polarity*-1);
        dir *= totalStrength;
        rb.AddForce(dir);
    }

    public bool AmIInRangeOfMagnet(GameObject other)
    {
        float dist = Vector2.Distance(transform.position, other.transform.position);

        if(other.GetComponent<Magnetic>() != null)
        {
            Magnetic magnet = other.GetComponent<Magnetic>();
            return dist <= magnet.range || dist <= range;
        }
        return false;
    }
}
