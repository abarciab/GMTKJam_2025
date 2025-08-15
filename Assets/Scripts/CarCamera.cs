using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("Car")]
    [SerializeField] private Vector2 _lookLimits = new Vector2(-30, 50);
    [SerializeField] private Transform _camParent;
    [SerializeField] private Rigidbody _carRb;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _targetOffset;
    [SerializeField] private float _turnLookAheadFactor = 2;
    [SerializeField] private float _posLerpFactor;
    [SerializeField] private float _rotLerpFactor;
    [SerializeField] private float _rotateSpeed = 1.1f;

    private Quaternion _carFreelookOffset;
    private Car _car;
    private float _currentLook = 0;
    private float _currentLookY = 0;
    private Transform _cam;
    private Quaternion _targetCameraLocalRotation;
    private Quaternion _targetRotation;
    private Vector3 _targetPosition;

    private Vector3 _targetLookPosition => _camParent.transform.position;
    public bool IsBoosting => _car.Boosting;
    public Quaternion TargetCameraLocalRotation => _targetCameraLocalRotation;
    public Quaternion TargetRotation => _targetRotation;
    public Vector3 TargetPosition => _targetPosition;

    private void Start()
    {
        _car = _carRb.GetComponent<Car>();
        _cam = Camera.main.transform;
    }

    private void Update()
    {
        _camParent.SetPositionAndRotation(_carRb.transform.TransformPoint(_targetOffset), _carRb.rotation * _carFreelookOffset);

        if (!Cursor.visible) DriveFreeLook();
        else _carFreelookOffset = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        _targetPosition = Vector3.Lerp(transform.position, _camParent.transform.TransformPoint(_positionOffset), _posLerpFactor * Time.deltaTime);
        CalculateTargetRotation();
    }

    private void CalculateTargetRotation()
    { 
        var oldRot = transform.rotation;
        //transform.LookAt(_camParent.transform);
        transform.LookAt(_targetLookPosition);
        //transform.LookAt(_camParent.transform.TransformPoint(_carTargetPosition));
        transform.localEulerAngles +=  _carRb.angularVelocity.y * _turnLookAheadFactor * Vector3.up;
        transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, _rotLerpFactor * Time.deltaTime);
        
        _targetRotation = transform.localRotation;
        transform.rotation = oldRot;

        _targetCameraLocalRotation = Quaternion.Lerp(_cam.localRotation, Quaternion.identity, _rotLerpFactor * Time.deltaTime);
    }

    private void DriveFreeLook()
    {
        var mouseDelta = -Input.mousePositionDelta.y * Utils.MouseSensitivity.y;
        var rotDelta = mouseDelta * _rotateSpeed * Time.deltaTime * 100;
        if (rotDelta > 0) rotDelta = Mathf.Min(rotDelta, _lookLimits.y - _currentLook);
        if (rotDelta < 0) rotDelta = Mathf.Max(rotDelta, _lookLimits.x - _currentLook);
        _currentLook += rotDelta;

        mouseDelta = Input.mousePositionDelta.x * Utils.MouseSensitivity.x;
        rotDelta = mouseDelta * _rotateSpeed * Time.deltaTime * 100;
        _currentLookY += rotDelta;

        _carFreelookOffset = Quaternion.Euler(_currentLook, _currentLookY, 0);
    }

    public void SnapToCar()
    {
        transform.position = _carRb.transform.TransformPoint(_positionOffset);
        transform.LookAt(_carRb.transform.TransformPoint(_targetOffset));
        transform.localEulerAngles += Vector3.up * _carRb.angularVelocity.y * _turnLookAheadFactor;
        _cam.localRotation = Quaternion.identity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, _targetLookPosition);
        Gizmos.DrawWireSphere(_targetLookPosition, 0.75f);
    }
}
