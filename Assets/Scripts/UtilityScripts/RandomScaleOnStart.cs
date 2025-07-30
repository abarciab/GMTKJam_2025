using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScaleOnStart : MonoBehaviour
{
    [SerializeField] private Vector3 _base;
    [SerializeField] private Vector2 _range;

    private void Start()
    {
        transform.localScale = _base * Random.Range(_range.x, _range.y);    
    }
}
