using System.Collections.Generic;
using UnityEngine;

public class MegaCreature : MonoBehaviour
{
    [SerializeField] private List<Foot> _feet;
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _resetDistance;
    [SerializeField] private Transform _body;

    private Vector3 _startingPosition;

    private void Start()
    {
        UpdateBodyPosition();
        _startingPosition = _body.position;
        foreach (Foot foot in _feet) foot.InitialWaitTime += _initialDelay;
    }

    private void Update()
    {
        UpdateBodyPosition();
    }

    private void UpdateBodyPosition()
    {
        var total = new Vector3();
        foreach (var foot in _feet) total += foot.transform.position;
        var center = new Vector3(total.x / _feet.Count, transform.position.y, total.z / _feet.Count);
        _body.position = center;
    }
}
