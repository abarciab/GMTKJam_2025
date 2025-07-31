using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody), typeof(PlayerInventory))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField] private float _strafeSpeed = 5;
    [SerializeField] private float _spinSpeed = 5;

    [Header("Collecting")]
    [SerializeField] private float _collectionRange = 10;
    [SerializeField] private Sound _collectSound;

    private Rigidbody _rb;
    private PlayerInventory _inventory;
    private Collectable _hoveredCollectible;
    private Car _hoveredCar;
    private bool _dead;
    private bool _breaking;
    private float _timeBreaking;

    public bool Dead => _dead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inventory = GetComponent<PlayerInventory>();
        _collectSound = Instantiate(_collectSound);
    }

    void Update()
    {
        if (_dead) return;

        Move();
        CollectableRaycast();

        if (_breaking) ProgressBreak();

        if (Input.GetMouseButtonDown(0))  {
            if (_hoveredCollectible != null) CollectableInteract();
            if (_hoveredCar != null) CarInteract(); 
        }
    }

    private void ProgressBreak()
    {
        if (_hoveredCollectible == null || Input.GetMouseButtonUp(0)) {
            _breaking = false;
            UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, 0);
            return;
        }

        _timeBreaking += Time.deltaTime;
        UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, _timeBreaking / _hoveredCollectible.BreakTime);

        if (_timeBreaking >= _hoveredCollectible.BreakTime) {
            CollectCurrent();
            _breaking = false;
            UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, 0);
        }
    }

    private void CarInteract()
    {
        if (_hoveredCar.ReadyToGo) LeaveArea();
        else DepositRocks();
    }

    private void LeaveArea()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Die()
    {
        _dead = true;
    }

    private void DepositRocks()
    {
        var requiredItems = _hoveredCar.GetRemainingRequiredItems();
        var ItemsToDeposit = _inventory.Inventory.GetOverlap(requiredItems);

        _hoveredCar.DepositItems(ItemsToDeposit);

        _inventory.RemoveItems(ItemsToDeposit);

        UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCar.GetDisplayString(_inventory.Inventory));
    }

    private void CollectableInteract()
    {
        if (_hoveredCollectible.Breakable) {
            _breaking = true;
            _timeBreaking = 0;
        }
        else CollectCurrent();
    }

    private void CollectCurrent()
    {
        if (_hoveredCollectible == null) return;

        _collectSound.Play();
        
        _inventory.Additems(_hoveredCollectible.Drop);

        Destroy(_hoveredCollectible.gameObject);
        _hoveredCollectible = null;
    }

    private void CollectableRaycast()
    {
        var cam = Camera.main.transform;
        var didHit = Physics.Raycast(cam.position, cam.forward, out var hitInfo, _collectionRange);
        if (!didHit) {
            UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, "");
            _hoveredCollectible = null;
            return;
        }

        _hoveredCollectible = hitInfo.collider.GetComponent<Collectable>();
        _hoveredCar = hitInfo.collider.GetComponent<Car>();

        if (_hoveredCollectible) UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCollectible.DisplayName);
        else if (_hoveredCar) {
            UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCar.GetDisplayString(_inventory.Inventory));
        }
        else UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, "");
    }

    private void Move()
    {
        UpdatePosition();
        Turn();
    }

    private void UpdatePosition()
    {
        if (InputController.Get(Control.MOVE_FORWARD)) {

            var targetSpeed = InputController.Get(Control.SPRINT) ? _runSpeed : _walkSpeed;

            var forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);
            if (forwardSpeed < targetSpeed) {
                _rb.linearVelocity += transform.forward * _walkSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_BACK)) {
            var backwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward * -1);
            if (backwardSpeed < _walkSpeed) {
                _rb.linearVelocity += transform.forward * -1 * _walkSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_RIGHT)) {
            var strafeSpeedRight = Vector3.Dot(_rb.linearVelocity, transform.right);
            if (strafeSpeedRight < _strafeSpeed) {
                _rb.linearVelocity += transform.right * _strafeSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_LEFT)) {
            var strafeSpeedLeft = Vector3.Dot(_rb.linearVelocity, transform.right * -1);
            if (strafeSpeedLeft < _strafeSpeed) {
                _rb.linearVelocity += transform.right * -1 * _strafeSpeed;
            }
        }
    }

    private void Turn()
    {
        var mouseDelta = Input.mousePositionDelta.x;

        transform.Rotate(Vector3.up, mouseDelta * _spinSpeed * Time.deltaTime * 100);
    }
}
