using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CarStatType { ACCEL, EFFICIENCY, TOP_SPEED, HANDLING, HP, CAPACITY}

[System.Serializable]
public class CarStat
{
    [HideInInspector] public string Name;
    public CarStatData Data;
    public float Value;
    public int Level;

    public List<Item> ItemsRequired => Data.CostForNextLevel(Level);
}

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [SerializeField] private float _maxFuel; 
    [SerializeField] private float _fuelUseFactor;
    [SerializeField] private Vector3 _playerDismountPosition;
    [SerializeField] private CarWorldUI _worldUI;

    [Header("Driving Mechanics")]
    [SerializeField] private Transform _model;
    [SerializeField] private float _modelLerpFactor;
    [SerializeField] private List<CarStat> _stats;
    [SerializeField] private Animator _carAnimator;
    [SerializeField] private float _gravity = 10;
    [SerializeField] private float _clampDownYPosThreshold = 0.1f;
    [SerializeField] private float _uprightLerpFactor = 4;
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _forwardAccel = 1;
    [SerializeField] private float _wheelTurnSpeed = 25;
    [SerializeField] private float _carTurnSpeed = 8;
    [SerializeField] private float _wheelTurnLimit = 22;
    [SerializeField] private Vector2 _throttleLimits = new Vector2(-3, 3);
    [SerializeField] private float _wheelStraightenLerpFactor = 22;
    [SerializeField] private Transform _leftTire;
    [SerializeField] private Transform _rightTire;
    [SerializeField] private LayerMask _floorLayerMask;
    [SerializeField, ReadOnly] private float _wheelAngle;

    [Header("Sounds")]
    [SerializeField] private Sound _engineLoop;

    [Header("Broken Behavior")]
    [SerializeField] private Inventory _currentInventory;
    [SerializeField] private Inventory _requiredInventory;
    [SerializeField] private Sound _depositSound;

    [Header("Misc")]
    [SerializeField] private Player _player;

    [SerializeField, ReadOnly] private float _currentFuel;
    private Rigidbody _rb;
    private bool _driving;
    private float _throttle;
    private float _forwardSpeedPercent;

    public bool ReadyToGo => _currentInventory.Contains(_requiredInventory);
    public void SetFuel(float fuel) => _currentFuel = fuel;
    public void SetThrottle(float throttle) => _throttle = throttle;
    public void setWheelAngle(float wheelAngle) => _wheelAngle = wheelAngle;
    public void SetEndGate(Transform endGate) => _worldUI.EndGate = endGate;
    public CarStat GetStat(CarStatType type) => _stats.Where(x => x.Data.Type == type).First();

    private void OnValidate()
    {
        foreach (var stat in _stats) if (stat.Data) stat.Name = stat.Data.DisplayName;
    }

    private void Awake()
    {
        _depositSound = Instantiate(_depositSound);
        _engineLoop = Instantiate(_engineLoop);
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _engineLoop.PlaySilent();
        UpdateUI();
        _rb.isKinematic = true;
    }

    private void Update()
    {
        _carAnimator.speed = _driving ? Mathf.Min(_currentFuel/2, 1) : 0;
        _worldUI.gameObject.SetActive(_driving);

        if (!_driving) {
            _engineLoop.SetPercentVolume(0, 0.5f * Time.deltaTime);
            return;
        }
        else {
            if (Input.GetKeyDown(KeyCode.T)) _currentFuel = _maxFuel;
        }

        if (InputController.GetDown(Control.INTERACT)) {
            LeaveCar();
            return;
        }

        _currentFuel -= Mathf.Max(_throttle, 2f) * _fuelUseFactor * Time.deltaTime;
        UIManager.i.Do(UIAction.SHOW_CAR_FUEL, (_currentFuel / _maxFuel));

        var forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);
        _forwardSpeedPercent = forwardSpeed / _maxSpeed;

        
        HandleThrottle();
        HandleTurning();       
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(_playerDismountPosition), 0.5f);
    }

    private void LeaveCar()
    {
        UpdateUI();

        foreach (var part in _model.GetComponentsInChildren<CarPart>()) {
            part.GetComponent<Collider>().enabled = true;
        }

        _rb.isKinematic = true;
        _throttle = 0;
        _wheelAngle = 0;
        UIManager.i.Do(UIAction.HIDE_CAR_HUD);
        _driving = false;
        FindFirstObjectByType<CameraController>().FollowPlayer();
        _player.transform.position = transform.TransformPoint(_playerDismountPosition);
        _player.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _engineLoop.SetPercentVolume(0.1f);
    }

    private void HandleThrottle()
    {
        if (InputController.Get(Control.MOVE_FORWARD) && _currentFuel > 0) {
            _throttle += _forwardAccel * Time.deltaTime;
        }
        else if (InputController.Get(Control.MOVE_BACK) && _currentFuel > 0) {
                _throttle -= _forwardAccel * Time.deltaTime;
        }
        else {
            if (_throttle > 0) _throttle -= _forwardAccel * Time.deltaTime;
            else if (_throttle < 0) _throttle += _forwardAccel * Time.deltaTime;
        }
        _throttle = Mathf.Clamp(_throttle, _throttleLimits.x, _throttleLimits.y);

        if (_currentFuel > 0) _engineLoop.SetPercentVolume(Mathf.Max(0.3f, Mathf.Abs(_throttle / _throttleLimits.y)));
        else _engineLoop.SetPercentVolume(0, 2 * Time.deltaTime);
    }

    private void HandleTurning()
    {
        if (InputController.Get(Control.MOVE_RIGHT)) {
            _wheelAngle += _wheelTurnSpeed * Time.deltaTime;
        }
        else if (InputController.Get(Control.MOVE_LEFT)) {
            _wheelAngle -= _wheelTurnSpeed * Time.deltaTime;
        }
        else {
            _wheelAngle = Mathf.Lerp(_wheelAngle, 0, Time.deltaTime * _wheelStraightenLerpFactor);
        }

        _wheelAngle = Mathf.Clamp(_wheelAngle, -_wheelTurnLimit, _wheelTurnLimit);
    }

    private void FixedUpdate()
    {
        if (_driving) Drive();

        var up = transform.localEulerAngles;
        up.z = 0;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(up), _uprightLerpFactor * Time.fixedDeltaTime);

        if (_driving) {
            _model.SetParent(null);
            _model.position = Vector3.Lerp(_model.position, transform.position, _modelLerpFactor * Time.fixedDeltaTime);
            _model.rotation = Quaternion.Lerp(_model.rotation, transform.rotation, _modelLerpFactor * Time.fixedDeltaTime);
        }
        else {
            _model.SetParent(transform);
            _model.localPosition = Vector3.zero;
            _model.localRotation = Quaternion.identity;
        }

    }

    private void Drive()
    {
        MoveCar();
        TurnTires();
    }

    private void TurnTires()
    {
        _leftTire.transform.localEulerAngles = new Vector3(0, 0, _wheelAngle);
        _rightTire.transform.localEulerAngles = new Vector3(0, 0, _wheelAngle);
    }

    private void MoveCar()
    {
        var forwardDir = transform.forward;
        forwardDir.y = 0;

        _rb.linearVelocity = _throttle * _forwardAccel * 10 * Time.fixedDeltaTime * forwardDir;
        _rb.angularVelocity = _carTurnSpeed * (_wheelAngle / _wheelTurnLimit) * Vector3.up;

        
        _rb.linearVelocity += Vector3.down * _gravity;
    }

    public void StartDriving()
    {
        foreach (var part in GetComponentsInChildren<CarPart>()) {
            part.GetComponent<Collider>().enabled = false;
        }

        _currentInventory.Subtract(_requiredInventory);

        _driving = true;
        _rb.isKinematic = false;
    }

    public List<Item> GetRemainingRequiredItems()
    {
        return _currentInventory.GetDifference(_requiredInventory);
    }

    public string GetDisplayString(Inventory playerInventory)
    {
        if (ReadyToGo) return "Fuel Full";
        else {
            var readyToDeposit = playerInventory.GetOverlap(GetRemainingRequiredItems()).Count > 0;
            if (readyToDeposit) return "Deposit Fuel";
            else return "More fuel required";
        }
    }

    public void DepositItems(List<Item> toDeposit)
    {
        foreach (var i in toDeposit) {
            _currentInventory.AddItems(i);
        }

        _depositSound.Play();

        if (GetRemainingRequiredItems().Count == 0) _currentFuel = _maxFuel;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.i.Do(UIAction.DISPLAY_CAR_INVENTORY, GetRemainingRequiredItems());
    }
}
