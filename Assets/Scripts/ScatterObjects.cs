using MyBox;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScatterObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();
    [SerializeField] private int _numObjects = 50;
    [SerializeField] private float _radius = 50;
    [SerializeField] private Vector2 _scaleLimits = new Vector2(0.4f, 1.6f);
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _permantentParent;

    [SerializeField] private List<float> _prefabProbabilities = new List<float>();

    private List<GameObject> _spawnedObjects = new List<GameObject>();

    private void OnValidate()
    {
        // Ensure the probabilities list matches the number of prefabs
        if (_prefabProbabilities.Count != _prefabs.Count)
        {
            while (_prefabProbabilities.Count < _prefabs.Count)
            {
                _prefabProbabilities.Add(1f); // Default probability
            }
            while (_prefabProbabilities.Count > _prefabs.Count)
            {
                _prefabProbabilities.RemoveAt(_prefabProbabilities.Count - 1);
            }
        }
    }

    private GameObject GetRandomPrefab()
    {
        if (_prefabs.Count == 0 || _prefabProbabilities.Count == 0) return null;

        // Normalize probabilities
        float totalProbability = 0f;
        foreach (var probability in _prefabProbabilities)
        {
            totalProbability += probability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        for (int i = 0; i < _prefabs.Count; i++)
        {
            cumulativeProbability += _prefabProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return _prefabs[i];
            }
        }

        return null; // Fallback (shouldn't happen)
    }

    [ButtonMethod]
    public void GenerateObjects()
    {
        DestroyObjects();

        for (int i = 0; i < _numObjects; i++) {
            TrySpawnObject();
        }
        
        Utils.SetDirty(this);
    }

    [ButtonMethod]
    public void DestroyObjects()
    {
        foreach (var obj in _spawnedObjects) DestroyImmediate(obj);
        for (var i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        _spawnedObjects.Clear();
        Utils.SetDirty(this);
    }

    [ButtonMethod]
    public void Confirm()
    {
        for (var i = transform.childCount - 1; i >= 0; i--) {
            var child = transform.GetChild(i);
            child.SetParent(_permantentParent);
            Utils.SetDirty(child.gameObject);   
        }
        _spawnedObjects.Clear();
        Utils.SetDirty(this);
        Utils.SetDirty(_permantentParent.gameObject);
    }

    [ButtonMethod]
    public void DestroyAll()
    {
        DestroyObjects();
        for (var i = _permantentParent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(_permantentParent.GetChild(i).gameObject);
        }
        Utils.SetDirty(this);
        Utils.SetDirty(_permantentParent.gameObject);
    }

    private void TrySpawnObject()
    {
        var circlePos = Random.insideUnitCircle * _radius;
        var pos = new Vector3(circlePos.x + transform.position.x, transform.position.y, circlePos.y + transform.position.z);

        var didHit = Physics.Raycast(pos, Vector3.down, out var hitInfo, 10000, _groundLayer);
        if (didHit)
        {
            var selectedObject = GetRandomPrefab();
            if (selectedObject != null)
            {
                var newObject = Instantiate(selectedObject, hitInfo.point, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                newObject.transform.localScale = Vector3.one * Random.Range(_scaleLimits.x, _scaleLimits.y);
                _spawnedObjects.Add(newObject);
                Utils.SetDirty(newObject);
            }
        }
        else {
            //var selectedObject = _prefabs[Random.Range(0, _prefabs.Count)];
            //var newObject = Instantiate(selectedObject, pos, Quaternion.identity, transform);
            //Utils.SetDirty(newObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius); 
    }

}
