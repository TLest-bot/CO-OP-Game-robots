using UnityEngine;
using System.Collections;

public class ButtonTrigger : MonoBehaviour
{
    public bool triggered;
    public float timepass;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            StartCoroutine(HandleButtonPress());
        }
    }

    IEnumerator HandleButtonPress()
    {
        triggered = true;

        timepass += 100f;

        GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(2f);

        GetComponent<SpriteRenderer>().color = Color.yellow;

        triggered = false;
    }
}