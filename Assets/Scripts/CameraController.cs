using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _rotateSpeed;

    [Header("Car")]
    [SerializeField] private Transform _car;
    [SerializeField] private Vector3 _carPositionOffset;
    [SerializeField] private Vector3 _carTargetPosition;
    [SerializeField] private float _posLerpFactor;
    [SerializeField] private float _rotLerpFactor;

    [Header("Misc")]
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector2 _lookLimits = new Vector2(-80, 40);

    private float _currentLook = 0;
    [SerializeField] private bool _followCar;

    private void Update()
    {
        if (_followCar) {
            transform.position = Vector3.Lerp(transform.position, _car.TransformPoint(_carPositionOffset), _posLerpFactor * Time.deltaTime);
            var oldRot = transform.rotation;
            transform.LookAt(_car.TransformPoint(_carTargetPosition));
            transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, _rotLerpFactor * Time.deltaTime);
        }
        else {
            transform.position = _player.TransformPoint(_positionOffset);
            transform.LookAt(_player.TransformPoint(_targetPosition));
            Turn();
        }        
    }

    public void FollowCar()
    {
        _followCar = true;
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
