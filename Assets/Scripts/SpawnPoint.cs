using MyBox;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> _options = new List<GameObject>();
    void Start()
    {
        var selected = _options[Random.Range(0, _options.Count)];
        var spawnedObject = Instantiate(selected, transform.position, transform.rotation, transform.parent);
        spawnedObject.transform.SetLossyScale(transform.lossyScale);
        Destroy(gameObject);
    }
}
