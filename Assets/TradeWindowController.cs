using System.Linq;
using UnityEngine;

public class TradeWindowController : UIController
{
    [SerializeField] private Transform _requestParent;
    [SerializeField] private Transform _offerParent;
    [SerializeField] private SelectableItem _confirmButton;

    private Trade _trade;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.START_TRADE && arg is Trade trade) StartTrade(trade);
    }

    private void StartTrade(Trade trade)
    {
        trade = new Trade(trade);

        _trade = trade;
        gameObject.SetActive(true);
        GameManager.i.Player.SetFrozen(true);
        Utils.SetCursor(true);

        var requestitems = trade.Request.Items;
        var requestDisplays = _requestParent.GetComponentsInChildren<InventoryItemDisplay>(true).ToList();
        for (int i = 0; i < requestDisplays.Count; i++) {
            requestDisplays[i].gameObject.SetActive(i < requestitems.Count);
            if (requestDisplays[i].gameObject.activeInHierarchy) {
                requestDisplays[i].Initialize(requestitems[i]);
            }
        }

        var offerItems = trade.Offer.Items;
        var offerDisplays = _offerParent.GetComponentsInChildren<InventoryItemDisplay>(true).ToList();
        for (int i = 0; i < offerDisplays.Count; i++) {
            offerDisplays[i].gameObject.SetActive(i < offerItems.Count);
            if (offerDisplays[i].gameObject.activeInHierarchy) {
                offerDisplays[i].Initialize(offerItems[i]);
            }
        }

        var canAfford = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory.Contains(requestitems);
        _confirmButton.SetDisabled(!canAfford);
    }

    public void Cancel()
    {
        CloseWindow();
    }

    public void Confirm()
    {
        var requestitems = _trade.Request.Items;
        var offerItems = _trade.Offer.Items;

        var inventory = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory;

        inventory.AddItems(offerItems);
        inventory.RemoveItems(requestitems);

        CloseWindow();
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
        GameManager.i.Player.SetFrozen(false);
        Utils.SetCursor(false);
    }
}


