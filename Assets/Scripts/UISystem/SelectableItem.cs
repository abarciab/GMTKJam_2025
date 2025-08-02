using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Runtime.ExceptionServices;
using UnityEditor;
using TMPro;

public enum SelectableItemDataType { GRAPHIC, GAMEOBJECT, CANVASGROUP, SPRITE}
public enum ButtonState { NORMAL, HOVERED, SELECTED, DISABLED }
public enum ClickBehavior { NONE, SELECT, TOGGLE };

[System.Serializable] public abstract class ListWrapper { public abstract void Set(int index); }

[System.Serializable] public class ListWrapperGeneric<T> : ListWrapper
{
    [SerializeField] private T _normal;
    [SerializeField] private T _hovered;
    [SerializeField] private T _selected;
    [SerializeField] private T _disabled;

    private List<T> _list => new List<T>() {_normal, _hovered, _selected, _disabled };
    private UnityAction<T> _setter;

    public T Get(int index) => _list[index];
    public override void Set(int index) => _setter(Get(index));

    public ListWrapperGeneric(T defaultValue)
    {
        _normal = _hovered = _selected = _disabled = defaultValue;
    }

    public void OnValidate(UnityAction<T> setter)
    {
        _setter = setter;
    }
};

[System.Serializable]
public class SelectableItemData
{
    [HideInInspector] public string Name;
    [SerializeField] private SelectableItemDataType _type;

    [ConditionalField(nameof(_showGraphicSettings)), SerializeField] private Graphic _graphic;
    [ConditionalField(nameof(_showGraphicSettings)), SerializeField] private bool _useMaterials;
    [ConditionalField(nameof(_showGraphicColorSettings)), SerializeField] private ListWrapperGeneric<Color> _colors;
    [ConditionalField(nameof(_useMaterials)), SerializeField] private ListWrapperGeneric<Material> _materials;

    [ConditionalField(nameof(_showGOSettings)), SerializeField] private GameObject _gameObjectTarget;
    [ConditionalField(nameof(_showGOSettings)), SerializeField] private ListWrapperGeneric<bool> _activeStates;

    [ConditionalField(nameof(_showGroupSettings)), SerializeField] private CanvasGroup _canvasGroup;
    [ConditionalField(nameof(_showGroupSettings)), SerializeField] private ListWrapperGeneric<float> _alphaValues;

    [ConditionalField(nameof(_showSpriteSettings)), SerializeField] private Image _spriteTarget;
    [ConditionalField(nameof(_showSpriteSettings)), SerializeField] private ListWrapperGeneric<Sprite> _sprites;

    [SerializeField, HideInInspector] private bool _showGraphicSettings;
    [SerializeField, HideInInspector] private bool _showGraphicColorSettings;
    [SerializeField, HideInInspector] private bool _showGOSettings;
    [SerializeField, HideInInspector] private bool _showGroupSettings;
    [SerializeField, HideInInspector] private bool _showSpriteSettings;

    private ListWrapper _current;
    private object _valid;

    private bool _isGraphic => _type == SelectableItemDataType.GRAPHIC;
    private bool _isGameObject => _type == SelectableItemDataType.GAMEOBJECT;
    private bool _isGroup => _type == SelectableItemDataType.CANVASGROUP;
    private bool _isSprite => _type == SelectableItemDataType.SPRITE;
    private bool _isValid => _valid != null;

    public void OnValidate()
    {
        if ((_isGraphic && !_graphic) || (_isGameObject && !_gameObjectTarget) || (_isGroup && !_canvasGroup) || (_isSprite && !_spriteTarget)) Initialize();
        else SetCurrent();

        if (_isValid) {
            if (_isGraphic) {
                _colors.OnValidate((color) => _graphic.GetComponent<CanvasRenderer>().SetColor(color));
                _materials.OnValidate((material) => _graphic.material = material);
            }
            if (_isGameObject) _activeStates.OnValidate((state) => _gameObjectTarget.SetActive(state));
            if (_isGroup)_alphaValues.OnValidate((alpha) => _canvasGroup.alpha = alpha);
            if (_isSprite)_sprites.OnValidate((sprite) => _spriteTarget.sprite = sprite);
        }

        if (!_isGraphic) _useMaterials = false;
        _showGraphicSettings = _isGraphic;
        _showGOSettings = _isGameObject;
        _showGroupSettings = _isGroup;
        _showSpriteSettings = _isSprite;
        _showGraphicColorSettings = _showGraphicSettings && !_useMaterials;
    }

    private void SetCurrent()
    {
        if (_isGraphic) {
            Name = _graphic.gameObject.name;
            _current = _useMaterials ? _materials : _colors;
            _valid = _graphic;
        }
        if (_isGameObject) {
            Name = _gameObjectTarget.name;
            _current = _activeStates;
            _valid = _gameObjectTarget;
        }
        if (_isGroup) {
            Name = _canvasGroup.gameObject.name;
            _current = _alphaValues;
            _valid = _canvasGroup;
        }
        if (_isSprite) {
            Name = _spriteTarget.gameObject.name;
            _current = _sprites;
            _valid = _spriteTarget;
        }
    }

    private void Initialize()
    {
        _colors = new ListWrapperGeneric<Color>(Color.white);
        _materials = new ListWrapperGeneric<Material>(null);
        _activeStates = new ListWrapperGeneric<bool>(false);
        _alphaValues = new ListWrapperGeneric<float>(1);
    }

    public void Update(ButtonState state)
    {
        if (!_isValid) return;
        _current.Set((int)state);
    }
}

public class SelectableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{ 
    [Header("Main")]
    [SerializeField] private ClickBehavior _clickBehavior = ClickBehavior.SELECT;
    [SerializeField, ConditionalField(nameof(_clickBehavior), true, false, ClickBehavior.NONE)] private bool _selectOnMouseDown = false;
    [SerializeField, ConditionalField(nameof(_clickBehavior), true, false, ClickBehavior.NONE)] private bool _canToggleOff = true;

    [Header("Hover")]
    [SerializeField] private ClickBehavior _hoverBehavior = ClickBehavior.NONE;
    [SerializeField] private bool _hasHoverCooldown;
    [SerializeField, ConditionalField(nameof(_hasHoverCooldown))] private float _hoverCooldown = 0.05f;
    [SerializeField] private bool _hoverWhenSelected = true;
    [SerializeField] private bool _allowHoverEventWhenDisabled = false;
    [SerializeField] private bool _deHoverOnEnable = false; 

    [Header("Appearance")]
    [SerializeField] private List<SelectableItemData> _data = new List<SelectableItemData>();

    [Header("Animation")]
    [SerializeField] private bool _scaleWhenHovered;
    [SerializeField, ConditionalField(nameof(_scaleWhenHovered))] private Vector2 _scaleDefaultHovered = new Vector2(1, 1.1f);
    [SerializeField, ConditionalField(nameof(_scaleWhenHovered))] private float _hoverScaleLerp = 0.2f;
    [SerializeField] private bool _hasAnimation;
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private Animator _animator;
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private string _animationSelectedBool = "Selected";
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private string _animationHoveredBool = "Hovered";
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private string _animationDisabledBool = "Disabled";

    [Header("Sounds")]
    [SerializeField] private Sound _hoverSound;
    [SerializeField] private Sound _mouseDownSound;
    [SerializeField] private Sound _selectSound;
    [SerializeField] private Sound _deselectSound;

    [Header("Events")]
    public UnityEvent OnSelect;
    public UnityEvent OnHover;
    public UnityEvent OnEndHover;
    public UnityEvent OnDeselect;

    [Header("Debug")]
    [SerializeField] private bool _debugStateChange;

    private ButtonState _visualState = ButtonState.NORMAL;
    private float _hoverDisabledTimeLeft;
    private bool _disabled;
    private bool _hovered;
    private bool _clickedDown;

    public bool Selected { get; private set; }

    private void OnValidate()
    {
        ValidateAll();
        foreach (var d in _data) d.Update(ButtonState.NORMAL);
    }

    private void OnEnable()
    {
        ValidateAll();
        if (_deHoverOnEnable && _hovered) {
            _hovered = false;
            if (_disabled) SetVisuals(ButtonState.DISABLED);
            else if (Selected) SetVisuals(ButtonState.SELECTED);
            else SetVisuals(ButtonState.NORMAL);
            if (_scaleWhenHovered) transform.localScale = Vector3.one * _scaleDefaultHovered.x;
        }
    }

    private void Start()
    {
        InitializeSounds();
        ValidateAll();
        SetVisuals(_visualState);
    }

    private void Update()
    {
        if (_scaleWhenHovered) {
            var targetScale = _hovered ? _scaleDefaultHovered.y : _scaleDefaultHovered.x;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, _hoverScaleLerp);
        }

        _hoverDisabledTimeLeft -= Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_disabled) {
            if (_allowHoverEventWhenDisabled) OnHover.Invoke();
            return;
        }
        if (Selected && !_hoverWhenSelected) return;
        if (_hasHoverCooldown && _hoverDisabledTimeLeft > 0)return;
        if (_clickBehavior == ClickBehavior.TOGGLE && Selected && !_canToggleOff) return;

        _clickedDown = false;

        StartHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_disabled) {
            if (_allowHoverEventWhenDisabled) OnEndHover.Invoke();
            return;
        }
        if (_hovered) EndHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_disabled) return;
        if (_clickBehavior == ClickBehavior.TOGGLE && Selected && !_canToggleOff) return;

        if (_clickBehavior == ClickBehavior.SELECT) SetVisuals(ButtonState.SELECTED);
        else if (_clickBehavior == ClickBehavior.TOGGLE) {
            if (_visualState != ButtonState.SELECTED) SetVisuals(ButtonState.SELECTED);
            else SetVisuals(ButtonState.NORMAL);
        }

        if (_mouseDownSound) _mouseDownSound.Play();
        _clickedDown = true;

        if (_selectOnMouseDown) CompleteSelection();
    }

    public void OnPointerUp(PointerEventData eventData) => CompleteSelection();

    private void CompleteSelection()
    {
        if (_disabled || !_clickedDown) return;
        _clickedDown = false;

        if (_clickBehavior == ClickBehavior.SELECT) {
            Select();
            Deselect(true);
        }
        else if (_clickBehavior == ClickBehavior.TOGGLE) ToggleSelected();

        if (_disabled) SetVisuals(ButtonState.DISABLED);
        else if (_hovered && _hoverWhenSelected) SetVisuals(ButtonState.HOVERED);
    }

    public void SetDisabled(bool disabled)
    {
        _disabled = disabled;

        if (disabled) SetVisuals(ButtonState.DISABLED);
        else {
            if (Selected) SetVisuals(ButtonState.SELECTED);
            if (_hovered) SetVisuals(ButtonState.HOVERED);
            else SetVisuals(ButtonState.NORMAL);
        }
    }

    private void ValidateAll()
    {
        foreach (var d in _data) d.OnValidate();
    }

    private void InitializeSounds()
    {
        if (_hoverSound) _hoverSound = Instantiate(_hoverSound);
        if (_selectSound) _selectSound = Instantiate(_selectSound);
        if (_mouseDownSound) _mouseDownSound = Instantiate(_mouseDownSound);
        if (_deselectSound) _deselectSound = Instantiate(_deselectSound);
    }

    public void ToggleSelected()
    {
        if (_disabled) return;
        if (Selected) Deselect();
        else Select();
    }

    public void Select(bool silent = false, bool triggerEvent = true)
    {   
        Selected = true;
        if (_debugStateChange) print(gameObject.name + " selected");
        if (!silent && _selectSound && _selectSound.Instantialized) _selectSound.Play();

        if (triggerEvent) OnSelect.Invoke();
        SetVisuals(ButtonState.SELECTED);
    }

    public void Deselect(bool silent = false, bool triggerEvent = true)
    {
        if (!Selected) return;

        Selected = false;
        if (_debugStateChange) print(gameObject.name + " deselected");
        if (_deselectSound && !silent) _deselectSound.Play();

        if (triggerEvent) OnDeselect.Invoke();
        if (_hovered) SetVisuals(ButtonState.HOVERED);
        else SetVisuals(ButtonState.NORMAL);
    }

    private void StartHover()
    {
        _hovered = true;

        if (_debugStateChange) print(gameObject.name + " hovered");
        if (_hoverSound) _hoverSound.Play();
        if (_hoverBehavior == ClickBehavior.SELECT) Select();
        else if (_hoverBehavior == ClickBehavior.TOGGLE) ToggleSelected();
        else {
            OnHover.Invoke();
            SetVisuals(ButtonState.HOVERED);
        }
    }

    private void EndHover()
    {
        _hovered = false;
        if (_hasHoverCooldown) _hoverDisabledTimeLeft = _hoverCooldown;
        if (_debugStateChange) print(gameObject.name + " end hover");

        OnEndHover.Invoke();
        if (Selected) SetVisuals(ButtonState.SELECTED);
        else SetVisuals(ButtonState.NORMAL);
    }

    public void SetVisuals(ButtonState state)
    {
        if (_debugStateChange) print(gameObject.name + "Changing visuals: " + state);

        _visualState = state;
        foreach (var d in _data) d.Update(_visualState);
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (!_hasAnimation) return;
        _animator.SetBool(_animationSelectedBool, _visualState == ButtonState.SELECTED);
        _animator.SetBool(_animationHoveredBool, _visualState == ButtonState.HOVERED);
        _animator.SetBool(_animationDisabledBool, _visualState == ButtonState.DISABLED);
    }

}