using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody), typeof(PlayerInventory))]
public class Player : MonoBehaviour
{
    [SerializeField] private CameraController _camera;

    [Header("Bow")]
    [SerializeField] private BowController _bow;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _maxArrowHoldTime = 3;
    [SerializeField] private float _arrowForce = 10;
    [SerializeField] private Sound _bowShotSound;

    [Header("Sprinting")]
    [SerializeField] private float _staminaMax = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField] private float _staminaRefilSpeed = 0.25f;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _encumberedSpeed = 10;
    [SerializeField] private float _strafeSpeed = 5;
    [SerializeField] private float _spinSpeed = 5;
    [SerializeField] private float _gravity;
    [SerializeField] private float _gravityLerpFactor = 0.5f;

    [Header("Jump")]
    [SerializeField] private float _jumpCheckRadius; 
    [SerializeField] private float _jumpCheckYOffset;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int _groundLayer;

    [Header("Collecting")]
    [SerializeField] private float _collectionRange = 10;
    [SerializeField] private Sound _collectSound;

    private Rigidbody _rb;
    private PlayerInventory _inventory;
    private bool _dead;
    private bool _frozen;
    private bool _breaking;
    private bool _talking;
    private bool _sprintEnabled;
    private float _timeBreaking;
    private float _timeHoldingArrow;
    private bool _grounded;
    private float _gravityDelta;
    private float _staminaLeft;

    private Outline _currHoveredOutline;
    private Collectable _hoveredCollectible;
    private CarPart _hoveredCarPart;
    private NPC _hoveredNPC;

    public bool Dead => _dead;
    public void EndConversation() => _talking = false;
    public void SetFrozen(bool frozen) => _frozen = frozen;
    public bool Frozen => _frozen;
    public bool Sprinting => InputController.Get(Control.SPRINT) && _rb.linearVelocity.magnitude > 0 && !_inventory.Encumbered && _staminaLeft > 0 && _sprintEnabled;

    private void OnEnable()
    {
        _staminaLeft = _staminaMax;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inventory = GetComponent<PlayerInventory>();
        _collectSound = Instantiate(_collectSound);
        _bowShotSound = Instantiate(_bowShotSound);
    }

    private void OnDisable()
    {
        UIManager.i.Do(UIAction.SHOW_STAMINA, 0);
    }

    void Update()
    {
        if (InputController.GetUp(Control.SPRINT)) {
            if (_staminaLeft < _staminaMax / 2) _sprintEnabled = false;
        }

        if (Sprinting) {
            _staminaLeft -= Time.deltaTime;
            if (_staminaLeft < 0) _sprintEnabled = false;
        }
        else {
            _staminaLeft += Time.deltaTime * _staminaRefilSpeed;
            if (!_sprintEnabled && _staminaLeft > _staminaMax / 2) _sprintEnabled = true;
        }
        _staminaLeft = Mathf.Clamp(_staminaLeft, 0, _staminaMax);
        UIManager.i.Do(UIAction.SHOW_STAMINA, _staminaLeft / _staminaMax);

        if (_dead || _talking || _frozen) return;

        if (InputController.GetDown(Control.INVENTORY)) UIManager.i.Do(UIAction.TOGGLE_INVENTORY, _inventory.Inventory(InventoryType.PLAYER));

        HandleJump();

        HandleArrow();

        Move();
        Raycast();

        if (_breaking) ProgressBreak();

        if (Input.GetMouseButtonDown(0))  {
            if (_hoveredCollectible != null) CollectableInteract();
            if (_hoveredCarPart != null) CarInteract(); 
            if (_hoveredNPC != null) NPCInteract(); 
        }

        //GameObject currObj  = _currHoveredOutline?.gameObject;
        //if (currObj == null) { return; }
        //if (currObj != _hoveredCarPart?.gameObject &&
        //    currObj != _hoveredCollectible?.gameObject &&
        //    currObj != _hoveredNPC?.gameObject) 
        //{
        //    _currHoveredOutline.enabled = false;
        //    _currHoveredOutline = null;
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _jumpCheckYOffset, _jumpCheckRadius);
    }

    private void HandleJump()
    {
        var colliders = Physics.OverlapSphere(transform.position + Vector3.up * _jumpCheckYOffset, _jumpCheckRadius);
        _grounded = colliders.Where(x => x.gameObject.layer == _groundLayer).Count() > 0;

        if (InputController.GetDown(Control.JUMP) && _grounded) {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
            _grounded = false;
        }
    }

    private void HandleArrow()
    {
        if (Input.GetMouseButton(1)) _timeHoldingArrow += Time.deltaTime;
        if (Input.GetMouseButtonUp(1)) FireArrow();
        var arrowPercent = _timeHoldingArrow / _maxArrowHoldTime;
        UIManager.i.Do(UIAction.SHOW_ARROW_PROGRESS, arrowPercent);
        _bow.UpdateArrow(arrowPercent);
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

    private void ProgressBreak()
    {
        if (_hoveredCollectible == null || Input.GetMouseButtonUp(0)) {
            _breaking = false;
            UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, 0f);
            if (_hoveredCollectible) _hoveredCollectible.SetBreakProgress(0);
            return;
        }

        _hoveredCollectible.SetBreakProgress(_timeBreaking / _hoveredCollectible.BreakTime);
        _timeBreaking += Time.deltaTime;
        UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, _timeBreaking / _hoveredCollectible.BreakTime);

        if (_timeBreaking >= _hoveredCollectible.BreakTime) {
            CollectCurrent();
            _breaking = false;
            UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, 0);
        }
    }

    private void NPCInteract()
    {
        UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");

        if (!_hoveredNPC.Spoken) {
            _talking = true;
            _camera.StartConversation();
        }
        _hoveredNPC.StartInteraction();
    }

    private void CarInteract()
    {
        if (_hoveredCarPart.Part == CarPartType.DOOR) GetInCar();
        if (_hoveredCarPart.Part == CarPartType.REPAIR_REFUL) OpenRepairMenu();
        if (_hoveredCarPart.Part == CarPartType.HOOD) OpenUpgradeMenu();
        if (_hoveredCarPart.Part == CarPartType.BED) OpenCarInventory();
    }

    private void OpenCarInventory()
    {
        UIManager.i.Do(UIAction.TOGGLE_CAR_INVENTORY, _inventory.Inventory(InventoryType.CAR));
    }

    private void OpenUpgradeMenu()
    {
        UIManager.i.Do(UIAction.SHOW_CAR_UPGRADE);
    }

    public void GetInCar()
    {
        _camera.FollowCar();
        if (_hoveredCarPart) _hoveredCarPart.Car.StartDriving();
        else FindFirstObjectByType<Car>(FindObjectsInactive.Include).StartDriving();

        gameObject.SetActive(false);
        UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");
    }

    public void Die()
    {
        _dead = true;
    }

    private void OpenRepairMenu()
    {
        UIManager.i.Do(UIAction.OPEN_REPAIR_MENU);
        UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");
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

        var drops = _hoveredCollectible.GetDrops();
        foreach (var item in drops) {
            GameManager.i.DiscoverItem(item.Data.Type);
        }

        _inventory.Inventory(InventoryType.PLAYER).AddItems(drops);

        Destroy(_hoveredCollectible.gameObject);
        _hoveredCollectible = null;
    }

    private void Raycast()
    {
        var cam = Camera.main.transform;
        var didHit = Physics.Raycast(cam.position, cam.forward, out var hitInfo, _collectionRange);
        if (!didHit) {
            UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");
            _hoveredCollectible = null;
            _hoveredCarPart = null;
            _hoveredNPC = null;
            DisableOutline();
            return;
        }

        _hoveredCollectible = hitInfo.collider.GetComponentInParent<Collectable>();
        _hoveredCarPart = hitInfo.collider.GetComponent<CarPart>();
        _hoveredNPC = hitInfo.collider.GetComponentInParent<NPC>();

        CheckNewOutline();

        if (_hoveredCollectible) {
            OnHoverOutline(_hoveredCollectible.ActiveModelVariant);
            UIManager.i.Do(UIAction.DISPLAY_HOVERED, _hoveredCollectible.DisplayName);
        } else if (_hoveredCarPart) {
            OnHoverOutline(_hoveredCarPart.gameObject);
            if (_hoveredCarPart.Part == CarPartType.DOOR) UIManager.i.Do(UIAction.DISPLAY_HOVERED, "Enter Car");
            else if (_hoveredCarPart.Part == CarPartType.HOOD) UIManager.i.Do(UIAction.DISPLAY_HOVERED, "Upgrade Car");
            else if (_hoveredCarPart.Part == CarPartType.BED) UIManager.i.Do(UIAction.DISPLAY_HOVERED, "Open Trunk");
            else if (_hoveredCarPart.Part == CarPartType.REPAIR_REFUL) {
                UIManager.i.Do(UIAction.DISPLAY_HOVERED, "Repair and Refuel");
            }
        } else if (_hoveredNPC) {
            UIManager.i.Do(UIAction.DISPLAY_HOVERED, _hoveredNPC.HoverName);
            OnHoverOutline(_hoveredNPC.gameObject);
        } else {
            UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");
            return;
        }

    }

    private void CheckNewOutline()
    {
        if (_currHoveredOutline == null) return; 

        if (_currHoveredOutline.gameObject != _hoveredCollectible?.ActiveModelVariant &&
            _currHoveredOutline.gameObject != _hoveredCarPart?.gameObject &&
            _currHoveredOutline.gameObject != _hoveredNPC?.gameObject) 
            { DisableOutline(); }
    }
    private void OnHoverOutline(GameObject outlineObj)
    {
        Outline outline = outlineObj.GetComponent<Outline>();
        if (!outline) {
            outline = outlineObj.AddComponent<Outline>();
            outline.OutlineWidth = 10f;
        }
        outline.enabled = true;
        _currHoveredOutline = outline;
    }
    private void DisableOutline()
    {
        if(_currHoveredOutline == null) return;
        _currHoveredOutline.enabled = false;
        _currHoveredOutline = null;
    }
    private void Move()
    {
        UpdatePosition();
        Turn();
    }

    private void UpdatePosition()
    {
        var encumbered = _inventory.Encumbered;

        if (InputController.Get(Control.MOVE_FORWARD)) {

            var targetSpeed = Sprinting ? _runSpeed : _walkSpeed;
            if (encumbered) targetSpeed = _encumberedSpeed;

            var forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);
            if (forwardSpeed < targetSpeed) {
                _rb.linearVelocity += transform.forward * _walkSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_BACK)) {
            var backwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward * -1);
            if (backwardSpeed < (encumbered ? _encumberedSpeed : _walkSpeed) ) {
                _rb.linearVelocity += transform.forward * -1 * _walkSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_RIGHT)) {
            var strafeSpeedRight = Vector3.Dot(_rb.linearVelocity, transform.right);
            if (strafeSpeedRight < _strafeSpeed * (encumbered? 0.1f : 1)) {
                _rb.linearVelocity += transform.right * _strafeSpeed;
            }
        }

        if (InputController.Get(Control.MOVE_LEFT)) {
            var strafeSpeedLeft = Vector3.Dot(_rb.linearVelocity, transform.right * -1);
            if (strafeSpeedLeft < _strafeSpeed * (encumbered ? 0.1f : 1)) {
                _rb.linearVelocity += transform.right * -1 * _strafeSpeed;
            }
        }

        var speedDown = Vector3.Dot(_rb.linearVelocity, Vector3.down);
        _gravityDelta = Mathf.Lerp(_gravityDelta, speedDown < _gravity && speedDown >= 0.25f ? _gravity : 0, _gravityLerpFactor * Time.deltaTime);
        _rb.linearVelocity += Vector3.down * _gravityDelta * Time.deltaTime;
    }

    private void Turn()
    {
        var mouseDelta = Input.mousePositionDelta.x;

        transform.Rotate(Vector3.up, mouseDelta * _spinSpeed * Time.deltaTime * 100);
    }
}
