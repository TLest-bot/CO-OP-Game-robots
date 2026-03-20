using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    public List<BoxCollider2D> magneticColliders;
    public Vector2 totalDirection;
    [field: Header("Magnet attributes")]
    [field: SerializeField]
    public int polarity; //1 is postive polarity, -1 is negative polarity
    public bool isStatic;
    public float strength;
    public float range;
    public float distanceStrengthMultiplier = 2;
    public float minMultiplier = 1f;

    public Rigidbody2D rb;


    void Awake()
    {
        GameObject.Find("Magnets Manager").GetComponent<MagnetsManager>().UpdateMagnetColliderList();
    }

    void FixedUpdate()
    {
        totalDirection = new Vector2();
        foreach(BoxCollider2D col in magneticColliders)
        {
            if(rb != null)
            {
                float dist = Vector2.Distance(transform.position, col.gameObject.transform.position);
                if (AmIInRangeOfMagnet(col.gameObject, dist) && !isStatic)
                {
                    GetPulledTowards(col, dist);
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

    public void GetPulledTowards(BoxCollider2D col, float dist)
    {
        Magnetic magnet = col.gameObject.GetComponent<Magnetic>();

        Vector2 myPos = transform.position;
        Vector2 dir = ((myPos - GetClosestMagneticPoint(col)).normalized*-1);
        float totalStrength = (magnet.strength*magnet.polarity) * (strength*polarity*-1) * CalculateDistStrenghtMultiplier(dist);
        dir *= totalStrength;
        //rb.AddForce(dir);
        totalDirection += dir;
    }

    public bool AmIInRangeOfMagnet(GameObject other, float dist)
    {

        if(other.GetComponent<Magnetic>() != null)
        {
            Magnetic magnet = other.GetComponent<Magnetic>();
            return dist <= magnet.range || dist <= range;
        }
        return false;
    }

    public float CalculateDistStrenghtMultiplier(float dist)
    {
        float total;
        float relativeDist = 1 - (dist / range);
        total = relativeDist * distanceStrengthMultiplier;

        if(total <= minMultiplier) { total = minMultiplier; }

        return total;
    }
}
