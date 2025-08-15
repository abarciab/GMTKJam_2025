using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CarCamera))]
public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player _player;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Vector2 _lookLimits = new Vector2(-80, 40);

    [Header("Misc")]
    [SerializeField] private Vector2 _fovLimits;
    [SerializeField] private float _fovLerpFactor = 5;
    [SerializeField] private Transform _camera;

    private CarCamera _carCam;

    private float _currentLook = 0;
    private bool _followCar;
    private bool _inConversation;

    public void StartConversation() => _inConversation = true;
    public void EndConversation() => _inConversation = false;
    public void FollowPlayer() => _followCar = false;
    public void FollowCar() => _followCar = true;
    public void SnapToCar() => _carCam.SnapToCar();

    private void Awake()
    {
        _carCam = GetComponent<CarCamera>();
    }

    private void Update()
    {
        var fovTarget = _fovLimits.x;
        if (_followCar) {
            if (_carCam.IsBoosting) fovTarget = _fovLimits.y;
        }
        else {
            if (_player.Sprinting) fovTarget = _fovLimits.y;
        }

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fovTarget, _fovLerpFactor * Time.deltaTime);

        if (!_followCar && !_inConversation && !_player.Frozen) {
            Turn();
        }

        if (_followCar) {
            
        }
    }

    private void FixedUpdate()
    {
        if (_followCar) {
            _camera.localRotation = _carCam.TargetCameraLocalRotation;
            transform.localRotation = _carCam.TargetRotation;
            transform.position = _carCam.TargetPosition;
        }
        else if (!_inConversation && !Cursor.visible) {
            transform.position = _player.transform.TransformPoint(_positionOffset);
            transform.LookAt(_player.transform.TransformPoint(_targetPosition));
        }
    }

    [ButtonMethod]
    public void SnapToPlayer()
    {
        if (!_player) _player = FindFirstObjectByType<Player>();
        transform.position = _player.transform.TransformPoint(_positionOffset);
        transform.LookAt(_player.transform.TransformPoint(_targetPosition));
        Utils.SetDirty(transform);
    }

    private void Turn()
    {
        var mouseDelta = -Input.mousePositionDelta.y * Utils.MouseSensitivity.y;
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
        Gizmos.DrawLine(_player.transform.TransformPoint(_positionOffset), _player.transform.TransformPoint(_targetPosition));
    }
}
