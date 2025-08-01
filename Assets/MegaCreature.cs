using System.Collections.Generic;
using UnityEngine;

public class MegaCreature : MonoBehaviour
{
    [SerializeField] private List<Foot> _feet;
    [SerializeField] private float _initialDelay;

    private void Start()
    {
        foreach (Foot foot in _feet) foot.InitialWaitTime += _initialDelay;
    }

}
