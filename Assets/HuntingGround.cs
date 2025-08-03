using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuntingGround : MonoBehaviour
{
    [SerializeField] private GameObject _animalPrefab;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private int _targetPopulation;
    [SerializeField] private Vector2 _spawnTimeRange;
    [SerializeField] private Vector2 _scaleRange;

    private List<GameObject> _spawnedAnimals = new List<GameObject>();
    private float _waitTime;

    private void Update()
    {
        _spawnedAnimals = _spawnedAnimals.Where(x => x != null).ToList();   

        _waitTime -= Time.deltaTime;
        if (_waitTime <= 0 && _spawnedAnimals.Count < _targetPopulation) {
            _waitTime = Random.Range(_spawnTimeRange.x, _spawnTimeRange.y);
            SpawnAnimal();
        }
    }

    private void SpawnAnimal()
    {
        var pos = Random.insideUnitSphere * _spawnRadius;
        pos.y = 0;
        pos += transform.position;

        var newAnimal = Instantiate(_animalPrefab, pos, Quaternion.identity, transform);
        newAnimal.transform.localScale = Vector3.one * Random.Range(_scaleRange.x, _scaleRange.y);

        _spawnedAnimals.Add(newAnimal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }

}
