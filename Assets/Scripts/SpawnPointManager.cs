using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public List<Spawnpoint> Spawnpoints;

    public Spawnpoint GetSpawnpointByLevel(int targetLevel)
    {
        Spawnpoint foundPoint = Spawnpoints.FirstOrDefault(s => s.level == targetLevel);

        if (foundPoint == null)
        {
            Debug.LogWarning($"Level {targetLevel} not found in the spawnpoints list!");
        }

        return foundPoint;
    }
}