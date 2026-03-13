using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MagnetsManager : MonoBehaviour
{
    public List<BoxCollider2D> magnetColliders;
    void Start()
    {

    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This runs when the scene is fully loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        UpdateMagnetColliderList();
    }

    public void UpdateMagnetColliderList()
    {
        magnetColliders = new List<BoxCollider2D>();
        //Fill my own list with all object tagged with "Magnetic" or "Player"
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Magnetic"))
        {
            FillColliderList(obj);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            FillColliderList(obj);
        }



        //Fill each gameobject with the Magnetic script with my list excluding themselves
        foreach (BoxCollider2D col in magnetColliders)
        {
            if(col.gameObject.GetComponent<Magnetic>() != null)
            {
                col.gameObject.GetComponent<Magnetic>().magneticColliders = new List<BoxCollider2D>();
                foreach (BoxCollider2D col_ in magnetColliders)
                {
                    if (col_.gameObject != col.gameObject)
                    {
                        col.gameObject.GetComponent<Magnetic>().magneticColliders.Add(col_);
                    }
                }
            }
        }
    }
    public void FillColliderList(GameObject obj)
    {
        if (obj.GetComponent<BoxCollider2D>() != null)
        {
            magnetColliders.Add(obj.GetComponent<BoxCollider2D>());
        }
    }
}
