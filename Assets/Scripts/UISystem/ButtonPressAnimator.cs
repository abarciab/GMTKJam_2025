using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform _shadow;
    [SerializeField] private RectTransform _backing;
    [SerializeField] private float _lerpFactor = 0.2f;

    private Vector2 _startingPos = Vector2.one * Mathf.Infinity;
    private bool _initialized;

    private void OnEnable()
    {
        if (_backing && !_initialized) {
            _initialized = true;
            _startingPos = _backing.anchoredPosition; 
        }
        _backing.anchoredPosition = _startingPos;
    }

    private void Update()
    {
        if (!_shadow || !_backing || !_initialized) return;

        var targetPos = _shadow.gameObject.activeInHierarchy ? _startingPos : _shadow.anchoredPosition;
        _backing.anchoredPosition = Vector2.Lerp(_backing.anchoredPosition, targetPos, _lerpFactor);
    }
}
