using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CarCamera), typeof(WalkCamera))]
public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player _player;

    [Header("Misc")]
    [SerializeField] private float _rotLerpFactor = 10;
    [SerializeField] private Vector2 _fovLimits;
    [SerializeField] private float _fovLerpFactor = 5;
    [SerializeField] private Transform _camera;

    private CarCamera _carCam;
    private WalkCamera _walkCam;

    private bool _inConversation;
    private bool _followCar;

    public void StartConversation() => _inConversation = true;
    public void EndConversation() => _inConversation = false;
    public void FollowPlayer() => _followCar = false;
    public void FollowCar() => _followCar = true;
    public void SnapToCar() => _carCam.SnapToCar();
    public void SnapToPlayer() => _walkCam.SnapToPlayer();
    private void Awake()
    {
        _carCam = GetComponent<CarCamera>();
        _walkCam = GetComponent<WalkCamera>();
    }

    private void Update()
    {
        SetFOV();
    }

    private void SetFOV()
    {
        var fovTarget = _fovLimits.x;
        if (_followCar) {
            if (_carCam.IsBoosting) fovTarget = _fovLimits.y;
        }
        else {
            if (_player.Sprinting) fovTarget = _fovLimits.y;
        }

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fovTarget, _fovLerpFactor * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_followCar) {
            _camera.localRotation = Quaternion.Lerp(_camera.localRotation, Quaternion.identity, _rotLerpFactor * Time.deltaTime);
            transform.localRotation = _carCam.TargetRotation;
            transform.position = _carCam.TargetPosition;
        }
        else if (!_inConversation && !Cursor.visible) {
            _walkCam.UpdatePositionAndRotation();

            _camera.localRotation = _walkCam.TargetCamLocalRotation;
            transform.position = _walkCam.TargetPosition;
            transform.rotation = _walkCam.TargetRotation;
        }
    }
}
