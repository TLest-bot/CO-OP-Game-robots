using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    public Magnetic magnet;
    public GameObject obj;
    private bool activated = false;
    public bool toggle;
    private float players = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated == false && players == 0)
        {
            if(magnet != null)
            {
                magnet.activated = true;
            }

            obj.SetActive(true);
        }
        activated = true;
        players++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (toggle && activated && players == 1)
        {
            activated = false;

            if (magnet != null)
            {
                magnet.activated = false;
            }

            obj.SetActive(false);
        }
        players--;
    }
}
