using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private Vector3 _axis = new Vector3(0, 1, 0);
    [SerializeField] private bool _seperateSpeed;
    [SerializeField, ConditionalField(nameof(_seperateSpeed))] private Vector3 _seperateSpeeds;
    [SerializeField, ConditionalField(nameof(_seperateSpeed), inverse:true)] private float _speed;

    private const float speedMod = 10;

    private void Update()
    {
        var speed = (_seperateSpeed ? _seperateSpeeds : Vector3.one * _speed) * speedMod;
        var delta = new Vector3(_axis.x * speed.x, _axis.y * speed.y, _axis.z * speed.z);

        transform.Rotate(delta * Time.deltaTime);
    }
}
