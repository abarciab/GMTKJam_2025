using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
    [SerializeField] private float _amplitude = 1f;
    [SerializeField] private float _frequency = 0.5f;
    [SerializeField] private bool _fixedTime;

    private float _startY;

    private void Start()
    {
        _startY = transform.localPosition.y;
    }

    void Update()
    {
        float time = _fixedTime ? Time.fixedTime : Time.time;
        float input = time % (1/_frequency);

        var pos = transform.localPosition;
        pos.y = 0;
        pos.y += Mathf.Sin(input * 2 * Mathf.PI * _frequency) * _amplitude;
        pos.y /= 2;
        pos.y += _startY;

        transform.localPosition = pos;
    }

    private void OnDrawGizmosSelected()
    {
        var pos = transform.localPosition;
        if (Application.isPlaying) pos.y = _startY;

        Gizmos.DrawLine(pos + Vector3.up * (_amplitude / 2), pos + Vector3.down * (_amplitude/2));
    }
}
