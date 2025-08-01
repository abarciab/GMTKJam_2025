using MyBox;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [Header("Driving")]
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _forwardAccel = 1;
    [SerializeField] private float _wheelTurnSpeed = 25;
    [SerializeField] private float _carTurnSpeed = 8;
    [SerializeField] private float _wheelTurnLimit = 22;
    [SerializeField] private Vector2 _throttleLimits = new Vector2(-3, 3);
    [SerializeField] private float _wheelStraightenLerpFactor = 22;
    [SerializeField] private Transform _leftTire;
    [SerializeField] private Transform _rightTire;
    [SerializeField, ReadOnly] private float _wheelAngle;

    [Header("Sounds")]
    [SerializeField] private Sound _engineLoop;

    [Header("Broken Behavior")]
    [SerializeField] private Inventory _currentInventory;
    [SerializeField] private Inventory _requiredInventory;
    [SerializeField] private Sound _depositSound;

    private Rigidbody _rb;
    private bool _driving;
    [SerializeField, ReadOnly] private float _throttle;
    private float _forwardSpeedPercent;

    public bool ReadyToGo => _currentInventory.Contains(_requiredInventory);

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
        if (!_driving) return;

        var forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);
        _forwardSpeedPercent = forwardSpeed / _maxSpeed;

        HandleThrottle();
        HandleTurning();       
    }

    private void HandleThrottle()
    {
        if (InputController.Get(Control.MOVE_FORWARD)) {
            _throttle += _forwardAccel * Time.deltaTime;
        }
        else if (InputController.Get(Control.MOVE_BACK)) {
                _throttle -= _forwardAccel * Time.deltaTime;
        }
        else {
            if (_throttle > 0) _throttle -= _forwardAccel * Time.deltaTime;
            else if (_throttle < 0) _throttle += _forwardAccel * Time.deltaTime;
        }
        _throttle = Mathf.Clamp(_throttle, _throttleLimits.x, _throttleLimits.y);

        _engineLoop.SetPercentVolume(Mathf.Max(0.3f, Mathf.Abs(_throttle / _throttleLimits.y)));
    }

    private void HandleTurning()
    {
        if (InputController.Get(Control.MOVE_RIGHT)) {
            _wheelAngle += (_wheelAngle < 0 ? 2 : 1) * _wheelTurnSpeed * Time.deltaTime;
        }
        else if (InputController.Get(Control.MOVE_LEFT)) {
            _wheelAngle -= (_wheelAngle > 0 ? 2 : 1) * _wheelTurnSpeed * Time.deltaTime;
        }
        else {
            _wheelAngle = Mathf.Lerp(_wheelAngle, 0, _forwardSpeedPercent * Time.deltaTime * _wheelStraightenLerpFactor);
        }

        _wheelAngle = Mathf.Clamp(_wheelAngle, -_wheelTurnLimit, _wheelTurnLimit);
    }

    private void FixedUpdate()
    {
        if (_driving) Drive();
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
        _rb.linearVelocity = _throttle * _forwardAccel * 10 * Time.deltaTime * transform.forward;
        _rb.angularVelocity = _forwardSpeedPercent * _carTurnSpeed * (_wheelAngle / _wheelTurnLimit) * Vector3.up;
    }

    public void StartDriving()
    {
        _driving = true;
        _rb.isKinematic = false;
    }

    public List<Item> GetRemainingRequiredItems()
    {
        return _currentInventory.GetDifference(_requiredInventory);
    }

    public string GetDisplayString(Inventory playerInventory)
    {
        if (ReadyToGo) return "Leave Area";
        else {
            var readyToDeposit = playerInventory.GetOverlap(GetRemainingRequiredItems()).Count > 0;
            if (readyToDeposit) return "Deposit Items";
            else return "More items required";
        }
    }

    public void DepositItems(List<Item> toDeposit)
    {
        foreach (var i in toDeposit) {
            _currentInventory.Additems(i);
        }

        _depositSound.Play();

        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.i.Do(UIAction.DISPLAY_CAR_INVENTORY, GetRemainingRequiredItems());
    }
}
