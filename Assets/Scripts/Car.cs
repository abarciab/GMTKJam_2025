using MyBox;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [Header("Driving")]
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _forwardAccel = 1;
    [SerializeField] private float _turnSpeed = 1;
    [SerializeField] private float _turnSpeedMax = 1;
    [SerializeField] private Transform _leftTire;
    [SerializeField] private Transform _rightTire;
    [SerializeField] private float _maxTireRot = 35;
    [SerializeField] private float _turnLerp = 10;

    [Header("Broken Behavior")]
    [SerializeField] private Inventory _currentInventory;
    [SerializeField] private Inventory _requiredInventory;
    [SerializeField] private Sound _depositSound;


    private Rigidbody _rb;
    private bool _driving;
    [SerializeField, ReadOnly] private float _rightSteerAmount;
    [SerializeField, ReadOnly] private float _leftSteerAmount;

    public bool ReadyToGo => _currentInventory.Contains(_requiredInventory);

    private void Awake()
    {
        _depositSound = Instantiate(_depositSound);
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        UpdateUI();
        _rb.isKinematic = true;
    }

    private void Update()
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
        var rightTurnSpeedPercent = Vector3.Dot(_rb.angularVelocity, transform.up) / _turnSpeedMax;
        //rightTurnSpeedPercent *= 2;
        //rightTurnSpeedPercent -= 1;

        _leftTire.transform.localEulerAngles = new Vector3(0, rightTurnSpeedPercent * _maxTireRot, 0);
        _rightTire.transform.localEulerAngles = new Vector3(0, rightTurnSpeedPercent * _maxTireRot, 180);
    }

    private void MoveCar()
    {
        var forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);
        var speedPercent = forwardSpeed / _maxSpeed;
        if (InputController.Get(Control.MOVE_FORWARD)) {
            if (forwardSpeed < _maxSpeed) {
                _rb.linearVelocity += _forwardAccel * 10 * Time.deltaTime * transform.forward;
            }
        }

        var targetRightTurn = 0;
        if (InputController.Get(Control.MOVE_RIGHT)) targetRightTurn = 1;
        _rightSteerAmount = Mathf.Lerp(_rightSteerAmount, targetRightTurn, _turnLerp * Time.deltaTime);
        
        var rightTurnSpeed = Vector3.Dot(_rb.angularVelocity, transform.up);
        if (rightTurnSpeed < _turnSpeedMax) {
            _rb.angularVelocity += _rightSteerAmount * _turnSpeed * 10 * speedPercent * Time.deltaTime * transform.up;
        }

        var targetLeftTurn = 0;
        if (InputController.Get(Control.MOVE_LEFT)) targetLeftTurn = 1;
        _leftSteerAmount = Mathf.Lerp(_leftSteerAmount, targetLeftTurn, _turnLerp * Time.deltaTime);

        var leftTurnSpeed = Vector3.Dot(_rb.angularVelocity, transform.up * -1);
        if (leftTurnSpeed < _turnSpeedMax) {
            _rb.angularVelocity += _leftSteerAmount * -_turnSpeed * 10 * speedPercent * Time.deltaTime * transform.up;
        }
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
