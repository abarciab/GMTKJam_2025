using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopySlider : MonoBehaviour
{
    [SerializeField] private Slider _leadSlider;
    [SerializeField] private Slider _followSlider;
    [SerializeField] private bool _lerp;
    [SerializeField, ConditionalField(nameof(_lerp))] private float _lerpFactor = 10;
    [SerializeField, ConditionalField(nameof(_lerp))] private float _delayTime = 0.8f;
    [SerializeField] private float _offset;

    private float _timeWhenLeadChanged;
    private bool _lerping;
    private bool _shouldLerp;

    private void Start()
    {
        _leadSlider.onValueChanged.AddListener(UpdateFollow);
        _followSlider.value = _leadSlider.value;
    }

    private void Update()
    {
        if (!_shouldLerp) return;

        if (_lerping) {
            _followSlider.value = Mathf.Lerp(_followSlider.value, _leadSlider.value - _offset, _lerpFactor * Time.deltaTime);
            if (Mathf.Abs(_followSlider.value - (_leadSlider.value - _offset)) < 0.01f) {
                _lerping = false;
                _shouldLerp = false;
            }
        }

        if (Time.time - _timeWhenLeadChanged > _delayTime) _lerping = true;
    }

    private void UpdateFollow(float value)
    {
        if (!_lerp || _leadSlider.value > _followSlider.value) _followSlider.value = value;
        else {
            _timeWhenLeadChanged = Time.time;
            _shouldLerp = true;
            _lerping = false;
        }
    }
}
