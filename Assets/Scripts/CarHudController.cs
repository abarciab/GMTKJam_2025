using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarHudController : UIController
{
    [SerializeField] private Slider _fuelSlider;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private float _speedLerp = 2;

    private float _speed;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_CAR_FUEL && arg is float fuelPercent) ShowFuel(fuelPercent);
        if (action == UIAction.SHOW_CAR_SPEED && arg is float speed) ShowSpeed(speed);

        if (action == UIAction.SHOW_CAR_SPEED || action == UIAction.SHOW_CAR_FUEL) gameObject.SetActive(true);

        if (action == UIAction.HIDE_CAR_HUD ) gameObject.SetActive(false);
    }

    private void ShowSpeed(float speed)
    {
        _speed = Mathf.Lerp(_speed, speed, _speedLerp * Time.deltaTime);
        var rounded = Mathf.Round(speed);
        if (_speed % 1 > 0.5f) rounded += 0.5f;
        _speedText.text = rounded.ToString();
    }

    private void ShowFuel(float fuelPercent)
    {
        _fuelSlider.gameObject.SetActive(true);
        _fuelSlider.value = fuelPercent;
    }

}
