using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "spawnPointData", menuName = "Scriptable Objects/spawnPointData")]
public class SpawnPointData : ScriptableObject
{
    public List<GameObject> Prefabs;
}
