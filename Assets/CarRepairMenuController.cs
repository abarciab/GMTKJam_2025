using System.Collections.Generic;
using UnityEngine;

public class CarRepairMenuController : UIController
{
    [SerializeField] private CarRepairSubMenuPanel _fuelPanel;
    [SerializeField] private float _fuelReplaceAmount;
    [SerializeField] private CarRepairSubMenuPanel _repairPanel;
    [SerializeField] private float _repairAmount;

    private Car _car;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.OPEN_REPAIR_MENU) gameObject.SetActive(true);
    }

    private void Update()
    {
        if (InputController.GetDown(Control.INVENTORY)) gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Utils.SetCursor(true);
        GameManager.i.Player.SetFrozen(true);

        if (!_car) _car = FindFirstObjectByType<Car>();

        _fuelPanel.Initialize(_car.FuelPercent, this);
        _repairPanel.Initialize(_car.HpPercent, this);
    }

    private void OnDisable()
    {
        Utils.SetCursor(false);
        GameManager.i.Player.SetFrozen(false);
    }

    public void Purchase(CarRepairSubMenuPanel source)
    {
        if (source == _fuelPanel) {
            _car.AddFuel(_fuelReplaceAmount);
        }
        else {
            _car.Heal(_repairAmount);
        }

        _fuelPanel.Initialize(_car.FuelPercent, this);
        _repairPanel.Initialize(_car.HpPercent, this);
    }
}
