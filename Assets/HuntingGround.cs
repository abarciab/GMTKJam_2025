using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuntingGround : MonoBehaviour
{
    [Header("Animal")]
    [SerializeField] private GameObject _animalPrefab;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private int _targetPopulation;
    [SerializeField] private Vector2 _spawnTimeRange;
    [SerializeField] private Vector2 _scaleRange;

    [Header("fires")]
    [SerializeField] private GameObject _spiritFirePrefab;
    [SerializeField] private float _spiritFireRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _numFires;

    private List<GameObject> _spawnedAnimals = new List<GameObject>();
    private float _waitTime;

    private void Start()
    {
        for (int i = 0; i < _numFires; i++) {
            var pos = Random.onUnitSphere;
            pos.y = 0;
            pos = pos.normalized * _spiritFireRadius;
            pos += transform.position;

            var hitGound = Physics.Raycast(pos+ Vector3.up * 10, Vector3.down, out var hitInfo, 1000, _groundLayer);
            if (!hitGound) continue;

            pos.y = hitInfo.point.y + Random.Range(0, 1);

            Instantiate(_spiritFirePrefab, pos, Quaternion.Euler(Vector3.up * Random.Range(0, 360)), transform);

        }
    }

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spiritFireRadius);
    }

}
