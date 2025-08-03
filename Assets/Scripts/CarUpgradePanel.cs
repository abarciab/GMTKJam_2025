using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarUpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject _requirements;
    [SerializeField] private CarStatType _stat;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Slider _slider;
    [SerializeField] private SelectableItem _upgradeButton;
    [SerializeField] private List<InventoryItemDisplay> _requiredItemDisplay = new List<InventoryItemDisplay>();
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private GameObject _upgradeParent;
    [SerializeField] private GameObject _maxLevelParent;

    private Car _car;

    public void SetHover(bool hovered)
    {
        foreach (var item in _car.GetStat(_stat).ItemsRequired) {
            GameManager.i.DiscoverItem(item.Data.Type);
        }

        var items = _car.GetStat(_stat).ItemsRequired;
        for (int i = 0; i < _requiredItemDisplay.Count; i++) {
            if (i < items.Count) {
                _requiredItemDisplay[i].Initialize(items[i]);
            }
            else _requiredItemDisplay[i].gameObject.SetActive(false);
        }

        _requirements.SetActive(hovered);
    }

    private void OnEnable()
    {
        if (!_car) _car = FindFirstObjectByType<Car>();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        _requirements.SetActive(false);
        var currentStat = _car.GetStat(_stat);
        _nameText.text = currentStat.Data.DisplayName;
        _descriptionText.text = currentStat.Data.Description;
        _slider.value = currentStat.Value / currentStat.Data.MaxValue;

        _upgradeParent.SetActive(currentStat.Value != currentStat.Data.MaxValue);
        _maxLevelParent.SetActive(!_upgradeParent.activeInHierarchy);

        var upgradeAmountString = currentStat.Data.LevelUpgradeAmount + currentStat.Data.AmountSuffix;
       // _buttonText.text = "Upgrade (+" + upgradeAmountString + ")";

        _levelText.text = currentStat.Level + "-> " + (currentStat.Level + 1);

        var items = currentStat.ItemsRequired;
        for (int i = 0; i < _requiredItemDisplay.Count; i++) {
            if (i < items.Count) {
                _requiredItemDisplay[i].Initialize(items[i]);
            }
            else _requiredItemDisplay[i].gameObject.SetActive(false);
        }

        var inventory = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.COMBINED);
        var affordable = inventory.Contains(items);
        _upgradeButton.SetDisabled(!affordable);

        //print(_nameText.text + " affordable: " + affordable);
    }

    public void Upgrade()
    {
        var currentStat = _car.GetStat(_stat);

        var inventory = GameManager.i.Player.GetComponent<PlayerInventory>();
        inventory.RemoveCombinedItems(currentStat.ItemsRequired);

        _car.UpgradeStat(_stat);

        foreach (var panel in transform.parent.GetComponentsInChildren<CarUpgradePanel>()) {
            panel.UpdateDisplay();
        } 
    }

}
