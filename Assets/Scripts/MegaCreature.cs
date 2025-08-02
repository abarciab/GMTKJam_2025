using System.Collections.Generic;
using UnityEngine;

public class MegaCreature : MonoBehaviour
{
    [SerializeField] private List<Foot> _feet;
    [SerializeField] private float _initialDelay;
    [SerializeField] private Transform _body;

    private void Start()
    {
        foreach (Foot foot in _feet) foot.InitialWaitTime += _initialDelay;
    }

    private void Update()
    {
        var total = new Vector3();
        foreach (var foot in _feet) total += foot.transform.position;
        var center = new Vector3(total.x / _feet.Count, transform.position.y, total.z / _feet.Count);

        _body.position = center;

    }
}
