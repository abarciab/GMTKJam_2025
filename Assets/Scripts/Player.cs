using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody), typeof(PlayerInventory))]
public class Player : MonoBehaviour
{
    [SerializeField] private CameraController _camera;

    [Header("Bow")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _maxArrowHoldTime = 3;
    [SerializeField] private float _arrowForce = 10;
    [SerializeField] private Vector3 _arrowSpawnPos;
    [SerializeField] private Sound _bowShotSound;

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
    private CarPart _hoveredCarPart;
    private bool _dead;
    private bool _breaking;
    private float _timeBreaking;
    private float _timeHoldingArrow;

    public bool Dead => _dead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inventory = GetComponent<PlayerInventory>();
        _collectSound = Instantiate(_collectSound);
        _bowShotSound = Instantiate(_bowShotSound);
    }

    void Update()
    {
        if (_dead) return;

        if (Input.GetMouseButton(1)) _timeHoldingArrow += Time.deltaTime;
        if (Input.GetMouseButtonUp(1)) FireArrow();

        Move();
        CollectableRaycast();

        if (_breaking) ProgressBreak();

        if (Input.GetMouseButtonDown(0))  {
            if (_hoveredCollectible != null) CollectableInteract();
            if (_hoveredCarPart != null) CarInteract(); 
        }
    }

    private void FireArrow()
    {
        _bowShotSound.Play();
        var cam = Camera.main.transform;
        var arrow = Instantiate(_arrowPrefab, cam.position, cam.rotation);
        var force = Mathf.Clamp01(_timeHoldingArrow / _maxArrowHoldTime) * _arrowForce;
        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * force);

        _timeHoldingArrow = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(_arrowSpawnPos), 0.25f);
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
        if (_hoveredCarPart.Part == CarPartType.DOOR) GetInCar();
        if (_hoveredCarPart.Part == CarPartType.FUEL) DepositItems();
    }

    private void GetInCar()
    {
        _camera.FollowCar();
        _hoveredCarPart.Car.StartDriving();
        gameObject.SetActive(false);
        UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, "");
    }

    public void Die()
    {
        _dead = true;
    }

    private void DepositItems()
    {
        var requiredItems = _hoveredCarPart.Car.GetRemainingRequiredItems();
        var ItemsToDeposit = _inventory.Inventory.GetOverlap(requiredItems);

        _hoveredCarPart.Car.DepositItems(ItemsToDeposit);

        _inventory.RemoveItems(ItemsToDeposit);

        UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCarPart.Car.GetDisplayString(_inventory.Inventory));
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
        _hoveredCarPart = hitInfo.collider.GetComponent<CarPart>();

        if (_hoveredCollectible) UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCollectible.DisplayName);
        else if (_hoveredCarPart) {
            if (_hoveredCarPart.Part == CarPartType.DOOR) UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, "Enter Car");
            else if (_hoveredCarPart.Part == CarPartType.FUEL) {
                UIManager.i.Do(UIAction.DISPLAY_COLLECTABLE, _hoveredCarPart.Car.GetDisplayString(_inventory.Inventory));
            }
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
