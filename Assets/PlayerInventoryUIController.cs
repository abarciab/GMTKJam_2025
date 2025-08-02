using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInventoryUIController : UIController
{
    [SerializeField] private GameObject _tooltipParent;
    [SerializeField] private TextMeshProUGUI _tooltipText;

    private Player _player;

    public void EndHover() => _tooltipParent.SetActive(false);

    public void HoverItem(string tooltipText)
    {
        _tooltipParent.SetActive(true);
        _tooltipText.text = tooltipText;
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.TOGGLE_INVENTORY && arg is Inventory inventory) ToggleInventory(inventory);
    }

    private void ToggleInventory(Inventory inventory)
    {
        if (!_player) _player = GameManager.i.Player;

        if (_player.Frozen && !gameObject.activeInHierarchy) return;

        if (!gameObject.activeInHierarchy) ShowInventory(inventory);
        else CloseInventory();

        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void ShowInventory(Inventory inventory)
    {

        
        _player.SetFrozen(true);
        Utils.SetCursor(true);

        _tooltipParent.SetActive(false);
        var itemDisplays = GetComponentsInChildren<InventoryItemDisplay>().ToList();
        var allItems = GameManager.i.AllItems.OrderBy(x => !GameManager.i.ItemDiscovered(x.Type)).ToList();
        for (int i = 0; i < itemDisplays.Count; i++) {
            var item = inventory.getItem(allItems[i].Type);
            itemDisplays[i].Initialize(item, this);
        }

    }

    private void CloseInventory()
    {
        _player.SetFrozen(false);
        Utils.SetCursor(false);
    }
}
