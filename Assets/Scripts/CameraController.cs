using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector2 _lookLimits = new Vector2(-80, 40);

    private float _currentLook = 0;

    private void Update()
    {
        transform.position = _player.TransformPoint(_positionOffset);
        transform.LookAt(_player.TransformPoint(_targetPosition));

        Turn();
    }

    private void Turn()
    {
        var mouseDelta = -Input.mousePositionDelta.y;
        var rotDelta = mouseDelta * _rotateSpeed * Time.deltaTime * 100;
        if (rotDelta > 0) rotDelta = Mathf.Min(rotDelta, _lookLimits.y - _currentLook);
        if (rotDelta < 0) rotDelta = Mathf.Max(rotDelta, _lookLimits.x - _currentLook);
        _currentLook += rotDelta;

        var rot = Quaternion.Euler(_currentLook, 0, 0);
        _camera.localRotation = rot;
    }

    private void OnDrawGizmos()
    {
        if (!_player) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_player.TransformPoint(_positionOffset), _player.TransformPoint(_targetPosition));
    }
}
