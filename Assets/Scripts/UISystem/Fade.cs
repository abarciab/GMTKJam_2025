using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fade : UIController
{
    [SerializeField] private Gradient _fadeGradient;

    private Image _img;
    private float _fadeTimeLeft;
    private float _maxFadeTime;
    private bool _reverse;

    private void OnEnable()
    {
        if (!_img) _img = GetComponent<Image>();
    }

    private void Update()
    {
        if (_maxFadeTime <= 0.01f) EndFade();

        float progress = _fadeTimeLeft / _maxFadeTime;
        if (_reverse) progress = 1 - progress;
        _img.color = _fadeGradient.Evaluate(progress);
        _fadeTimeLeft -= Time.deltaTime;
        if (_fadeTimeLeft < 0) EndFade();
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.FADE_FROM_BLACK) SetVisibility(true);
        if (action == UIAction.FADE_TO_BLACK) SetVisibility(false);
    }

    private void EndFade()
    {
        enabled = false;
        _maxFadeTime = _fadeTimeLeft = 0;
        gameObject.SetActive(_reverse);
    }

    public void SetVisibility(bool appear, float time = -1)
    {
        if (time < 0) time = Utils.fadeTime;

        gameObject.SetActive(true);
        _img.raycastTarget = appear;
        _maxFadeTime = time;
        _reverse = !appear;
        _fadeTimeLeft = _maxFadeTime;
        enabled = true;
    }

}
