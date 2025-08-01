using MyBox;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnPointData _data;
    void Start()
    {
        var selected = _data.Prefabs[Random.Range(0, _data.Prefabs.Count)];
        var spawnedObject = Instantiate(selected, transform.position, transform.rotation, transform.parent);
        spawnedObject.transform.SetLossyScale(transform.lossyScale);
        Destroy(gameObject);
    }
}
