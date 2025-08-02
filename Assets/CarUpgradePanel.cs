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

    public void SetHover(bool hovered) => _requirements.SetActive(hovered);

    private void OnEnable()
    {
        var car = FindFirstObjectByType<Car>();

        var currentStat = car.GetStat(_stat);
        _nameText.text = currentStat.Data.DisplayName;
        _descriptionText.text = currentStat.Data.Description;
        _slider.value = currentStat.Value / currentStat.Data.MaxValue;

        var upgradeAmountString = currentStat.Data.LevelUpgradeAmount + currentStat.Data.AmountSuffix;
        _buttonText.text = "Upgrade (+" + upgradeAmountString + ")";

        var items = currentStat.ItemsRequired;
        for (int i = 0; i < items.Count; i++) {
            _requiredItemDisplay[i].Initialize(items[i]);
        }

        var affordable = (GameManager.i.Player.GetComponent<PlayerInventory>().Inventory.Contains(items));
        _upgradeButton.SetDisabled(!affordable);

    }
}
