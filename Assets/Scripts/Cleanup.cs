using UnityEngine;

public class Cleanup : MonoBehaviour
{
    public float Time;
    void Start()
    {
        Destroy(gameObject, Time);
    }
}