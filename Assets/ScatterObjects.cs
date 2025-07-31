using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScatterObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();
    [SerializeField] private int _numObjects = 50;
    [SerializeField] private float _radius = 50;
    [SerializeField] private Vector2 _scaleLimits = new Vector2(0.4f, 1.6f);
    [SerializeField] private LayerMask _groundLayer;

    private List<GameObject> _spawnedObjects = new List<GameObject>();

    private void Start()
    {
        GenerateObjects();
    }

    private void GenerateObjects()
    {
        foreach (var obj in _spawnedObjects) Destroy(obj);
        _spawnedObjects.Clear();

        for (int i = 0; i < _numObjects; i++) {
            TrySpawnObject();
        }
    }

    private void TrySpawnObject()
    {
        var circlePos = Random.insideUnitCircle * _radius;
        var pos = new Vector3(circlePos.x, transform.position.y, circlePos.y);

        var didHit = Physics.Raycast(pos, Vector3.down, out var hitInfo, 10000, _groundLayer);
        if (didHit) {
            var selectedObject = _prefabs[Random.Range(0, _prefabs.Count)];
            var newObject = Instantiate(selectedObject, hitInfo.point,Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            newObject.transform.localScale = Vector3.one * Random.Range(_scaleLimits.x, _scaleLimits.y);
            _spawnedObjects.Add(newObject);
        }
        else {
            var selectedObject = _prefabs[Random.Range(0, _prefabs.Count)];
            var newObject = Instantiate(selectedObject, pos, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius); 
    }

}
