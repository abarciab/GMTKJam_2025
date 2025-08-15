using System.Collections.Generic;
using UnityEngine;

public class HarrowedSpawner : MonoBehaviour
{
    [SerializeField] private Transform _creatureParent;
    [SerializeField, Range(0, 1)] private List<float> _spawnTimes;

    private List<float> _originalSpawnTimes;

    private void Start()
    {
        _originalSpawnTimes = new List<float>(_spawnTimes);
    }

    public void UpdatePercent(float timerPercent)
    {
        if (_spawnTimes.Count > 0 && timerPercent <= _spawnTimes[0]) {
            foreach (Transform child in _creatureParent) {
                if (!child.gameObject.activeInHierarchy) {
                    child.gameObject.SetActive(true);
                    break;
                }
            }
            _spawnTimes.RemoveAt(0);
        }
    }
    
    public void EnterNewArea()
    {
        _spawnTimes = new List<float>(_originalSpawnTimes);
        foreach (Transform child in _creatureParent) child.gameObject.SetActive(false);
    }
}
