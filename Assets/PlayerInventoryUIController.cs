using UnityEngine;

public class PlayerInventoryUIController : UIController
{
    private Player _player;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.TOGGLE_INVENTORY && arg is Inventory inventory) ToggleInventory(inventory);
    }

    private void ToggleInventory(Inventory inventory)
    {
        if (!gameObject.activeInHierarchy) ShowInventory(inventory);
        else CloseInventory();

        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void ShowInventory(Inventory inventory)
    {
        if (!_player) _player = GameManager.i.Player;
        _player.SetFrozen(true);
        Utils.SetCursor(true);
    }

    private void CloseInventory()
    {
        _player.SetFrozen(false);
        Utils.SetCursor(false);
    }
}
