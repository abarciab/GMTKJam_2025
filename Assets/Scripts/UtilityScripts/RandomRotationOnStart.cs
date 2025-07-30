using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationOnStart : MonoBehaviour
{
    [SerializeField] private Vector3 _base = Vector3.forward;

    void Start()
    {
        transform.localEulerAngles += _base * Random.Range(0, 360);
    }
}
