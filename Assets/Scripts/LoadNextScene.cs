using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLoadNextScene()
    {
        Debug.Log("Loading next level");
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Loading next level");
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }
}
