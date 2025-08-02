using UnityEngine;

public class CarUpgradeMenuController : UIController
{
    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_CAR_UPGRADE) OpenMenu();
    }

    private void Update()
    {
        if (InputController.GetDown(Control.INVENTORY)) CloseMenu();
    }

    private void OpenMenu()
    {
        GameManager.i.Player.SetFrozen(true);
        UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");

        Utils.SetCursor(true);
        gameObject.SetActive(true);
    }

    private void CloseMenu()
    {
        GameManager.i.Player.SetFrozen(false);
        Utils.SetCursor(false);
        gameObject.SetActive(false);
    }

}
