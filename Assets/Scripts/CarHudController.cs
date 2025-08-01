using UnityEngine;
using UnityEngine.UI;

public class CarHudController : UIController
{
    [SerializeField] private Slider _fuelSlider;

    private void Start()
    {
        _fuelSlider.gameObject.SetActive(false);
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_CAR_FUEL && arg is float fuelPercent) ShowFuel(fuelPercent);
        if (action == UIAction.HIDE_CAR_HUD ) _fuelSlider.gameObject.SetActive(false);
    }

    private void ShowFuel(float fuelPercent)
    {
        _fuelSlider.gameObject.SetActive(true);
        _fuelSlider.value = fuelPercent;
    }

}
