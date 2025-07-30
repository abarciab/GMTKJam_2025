using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    [SerializeField] private Sprite _uncheckedSprite;
    [SerializeField] private Sprite _checkedSprite;
    [SerializeField] private Image _checkboxImg;
    [SerializeField] private bool _checked = false;

    [SerializeField] private UnityEvent OnToggleOn;
    [SerializeField] private UnityEvent OnToggleOff;

    private void Start()
    {
        _checkboxImg.sprite = _checked ? _checkedSprite : _uncheckedSprite;
    }

    public void Toggle()
    {
        if (_checked) ToggleOff();
        else ToggleOn();
    }

    private void ToggleOn()
    {
        _checked = true;
        OnToggleOn.Invoke();
        _checkboxImg.sprite = _checkedSprite;
    }

    private void ToggleOff()
    {
        _checked = false;
        OnToggleOff.Invoke();
        _checkboxImg.sprite = _uncheckedSprite;
    }

}
