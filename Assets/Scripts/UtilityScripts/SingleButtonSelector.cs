using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleButtonSelector : MonoBehaviour
{
    List<SelectableItem> _buttons = new List<SelectableItem>();
    [SerializeField] private bool _selectOnEnable = false;
    [SerializeField, ConditionalField(nameof(_selectOnEnable))] private int _onEnableChildIndex = 0;

    bool initialized = false;

    private void OnEnable()
    {
        if (!initialized) Initialize();
        if (_selectOnEnable && _buttons.Count > 0) _buttons[_onEnableChildIndex].Select(true, true); 
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;
        _buttons = GetComponentsInChildren<SelectableItem>().ToList();
        foreach (var b in _buttons) b.OnSelect.AddListener(() => DeselectOthers(b));
        initialized = true;
    }

    private void DeselectOthers(SelectableItem selected)
    {
        foreach (var b in _buttons) if (selected != b) b.Deselect();
    }
}
