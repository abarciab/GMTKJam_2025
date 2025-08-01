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
    [SerializeField] private Vector2 _lookLimits = new Vector2(-80, 40);

    [Header("Car")]
    [SerializeField] private Rigidbody _car;
    [SerializeField] private Vector3 _carPositionOffset;
    [SerializeField] private Vector3 _carTargetPosition;
    [SerializeField] private float _turnLookAheadFactor = 2;
    [SerializeField] private float _posLerpFactor;
    [SerializeField] private float _rotLerpFactor;

    [Header("Misc")]
    [SerializeField] private Transform _camera;

    private float _currentLook = 0;
    [SerializeField] private bool _followCar;

    private void Update()
    {
        if (_followCar) {
        }
        else {
            Turn();
        }
    }

    private void FixedUpdate()
    {
        if (_followCar) {
            transform.position = Vector3.Lerp(transform.position, _car.transform.TransformPoint(_carPositionOffset), _posLerpFactor * Time.deltaTime);
            var oldRot = transform.rotation;
            transform.LookAt(_car.transform.TransformPoint(_carTargetPosition));
            transform.localEulerAngles += Vector3.up * _car.angularVelocity.y * _turnLookAheadFactor;
            transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, _rotLerpFactor * Time.deltaTime);
            

            _camera.localRotation = Quaternion.Lerp(_camera.localRotation, Quaternion.identity, _rotLerpFactor * Time.deltaTime);
        }
        else {
            transform.position = _player.TransformPoint(_positionOffset);
            transform.LookAt(_player.TransformPoint(_targetPosition));
        }
    }

    public void FollowCar()
    {
        _followCar = true;
    }

    public void FollowPlayer()
    {
        _followCar = false;
    }

    private void Turn()
    {
        var mouseDelta = -Input.mousePositionDelta.y;
        //mouseDelta = Mathf.Clamp(mouseDelta, -50, 50);
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
