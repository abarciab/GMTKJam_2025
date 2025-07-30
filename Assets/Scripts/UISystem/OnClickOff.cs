using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnClickOff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [OverrideLabel("OnClickOff")] public UnityEvent Event;
    [SerializeField] private bool _requireInitialHover = true;

    private bool _over;
    private bool _clicked;

    private void OnEnable()
    {
        if (!_requireInitialHover) {
            _clicked = false;
            _over = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _over = true;
        _clicked = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _over = false;
    }

    private void OnDisable()
    {
        _over = false;
    }

    private void OnDestroy()
    {
        _over = false;
    }

    private void Update()
    {
        if (_clicked || _over) return; 

        if (Input.GetMouseButtonDown(0)) {
            Event.Invoke();
            _clicked = true;
        }
    }
}
