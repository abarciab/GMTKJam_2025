using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryDisplayController : UIController
{
    [SerializeField] private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.TOGGLE_INVENTORY && arg is List<Item> items) DisplayInventory(items);
    }

    private void DisplayInventory(List<Item> items)
    {
        for (int i = 0; i < _texts.Count; i++) {
            _texts[i].gameObject.SetActive(items.Count > i && items[i].Quantity > 0); 
            if (_texts[i].gameObject.activeInHierarchy) {
                _texts[i].text = items[i].Data.DisplayName + ": " + items[i].Quantity;
            }
        }
    }
}
