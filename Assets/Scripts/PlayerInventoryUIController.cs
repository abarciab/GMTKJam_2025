using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUIController : UIController
{
    [SerializeField] private GameObject _tooltipParent;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private bool _isCar;
    [SerializeField] private Slider _weightLimitSlider;
    [SerializeField] private float _weightLimit = 15;

    private Player _player;
    private Inventory _inventory;

    public void EndHover() => _tooltipParent.SetActive(false);

    public void HoverItem(string tooltipText)
    {
        _tooltipParent.SetActive(true);
        _tooltipText.text = tooltipText;
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (!_isCar && action == UIAction.TOGGLE_INVENTORY && arg is Inventory inventory) ToggleInventory(inventory);
        if (_isCar && action == UIAction.TOGGLE_CAR_INVENTORY && arg is Inventory carInventory) ToggleInventory(carInventory);
    }

    private void Update()
    {
        if (InputController.GetDown(Control.INVENTORY)) CloseInventory();

        var totalItems = _inventory.Items.Sum(x => x.Quantity);
        var targetValue = totalItems / _weightLimit;
        _weightLimitSlider.value = Mathf.Lerp(_weightLimitSlider.value, targetValue, 10 * Time.deltaTime);
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
        _inventory = inventory;

        var totalItems = _inventory.Items.Sum(x => x.Quantity);
        _weightLimitSlider.value = totalItems / _weightLimit;        

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
        gameObject.SetActive(false);
        _player.SetFrozen(false);
        Utils.SetCursor(false);
    }
}
