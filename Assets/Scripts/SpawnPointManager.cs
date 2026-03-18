using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPointManager : MonoBehaviour
{
    public List<Spawnpoint> Spawnpoints;
    public GameObject objects;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public Spawnpoint GetSpawnpointByLevel(int targetLevel)
    {
        Spawnpoint foundPoint = Spawnpoints.FirstOrDefault(s => s.level == targetLevel);

        if (foundPoint != null)
        {
            return foundPoint;
        }
        else
        {
            Debug.LogWarning($"Level {targetLevel} niet gevonden in de spawnpoints lijst!");
            return null;
        }
    }
    public bool isplayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject player = this.gameObject;
        }
        {
            return true;
        }
    }
    public void Teleport(GameObject player,  Spawnpoint spawnpoint)
    {
        player.transform.position = spawnpoint.transform.position;
    }

    void ZeroButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(0)); }
        Debug.Log("button one pressed");
    }
    void OneButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(1)); }
        Debug.Log("button one pressed");
    }

    void TwoButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(2)); }
    }

    void ThreeButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(3)); }
    }

    void FourButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(4)); }
    }

    void FiveButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(5)); }
    }

    void SixButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(6)); }
    }

    void SevenButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(7)); }
    }

    void EightButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(8)); }
    }

    void NineButton(InputAction.CallbackContext context)
    {
        if (context.performed) { Teleport(this.gameObject, GetSpawnpointByLevel(9)); }
    }
}
