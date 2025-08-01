using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarInventoryDisplay : UIController
{
    [SerializeField] private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    [SerializeField] private GameObject _headerObject;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.DISPLAY_CAR_INVENTORY && arg is List<Item> requiredItems) DisplayInventory(requiredItems);
    }

    private void DisplayInventory(List<Item> requiredItems)
    {
        for (int i = 0; i < _texts.Count; i++) {
            _texts[i].gameObject.SetActive(requiredItems.Count > i);
            if (_texts[i].gameObject.activeInHierarchy) {
                _texts[i].text = requiredItems[i].Data.DisplayName + ": " + requiredItems[i].Quantity;
            }
        }
        _headerObject.SetActive(requiredItems.Count > 0);
    }
}
