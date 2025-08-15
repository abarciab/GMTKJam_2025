using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryItemDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _quantityText;
    [SerializeField] private GameObject _quantityParent;
    [SerializeField] private bool _redWhenUnaffordable;
    [SerializeField] private bool _showUnknown;
    [SerializeField] private bool _allowDiscard;
    [SerializeField] private bool _trunk;
    [SerializeField] private bool _combinedQuantityCheck;
    [SerializeField] private SelectableItem _quantity;
    [SerializeField] private GameObject _discardButton;
    [SerializeField] private GameObject _iconFrameParent;
    [SerializeField] private GameObject _trunkButtonParent;

    [Header("Buttons")]
    [SerializeField] private GameObject _takeButton;
    [SerializeField] private GameObject _takeAllButton;
    [SerializeField] private GameObject _giveButton;
    [SerializeField] private GameObject _giveAllButton;

    private PlayerInventoryUIController _controller;
    private Item _item;
    private bool _discovered;

    public void EndHover() => _controller?.EndHover();
    public void Initialize(Item item, PlayerInventoryUIController controller)
    {
        _controller = controller;
        Initialize(item);
    }

    public void Initialize(Item item)
    {
        gameObject.SetActive(true);

        var itemData = InventoryManager.GetData(item.Type);

        _item = item;
        _icon.sprite = itemData.Icon;

        _discovered = InventoryManager.i.ItemDiscovered(itemData.Type) || _showUnknown;
        if (_discovered) _nameText.text = itemData.DisplayName;
        else _nameText.text = "???";

        if (_trunkButtonParent) {
            _trunkButtonParent.SetActive(_trunk && _discovered);

            if (_trunk) {
                var carInv = InventoryManager.i.Inventory(InventoryType.CAR);
                var playerInv = InventoryManager.i.Inventory(InventoryType.PLAYER);

                var carCount = carInv.GetCount(itemData.Type);
                _takeButton.SetActive(carCount > 0);
                _takeAllButton.SetActive(carCount > 1);

                var playerCount = playerInv.GetCount(itemData.Type);
                _giveButton.SetActive(playerCount > 0);
                _giveAllButton.SetActive(playerCount > 1);
            }
        }

        _quantityParent.SetActive(_discovered);
        _quantityText.text = item.Quantity.ToString();

        if (_quantity) {
            var player = GameManager.i.Player;
            var inventory = _combinedQuantityCheck ? InventoryManager.i.Inventory(InventoryType.COMBINED) : InventoryManager.i.Inventory(InventoryType.PLAYER);
            if (_redWhenUnaffordable && inventory.GetCount(item.Type) < item.Quantity) {
                _quantity.SetDisabled(true);
            }
            else {
                _quantity.SetDisabled(false);
            }
        }

        if (_discardButton) _discardButton.SetActive(_allowDiscard && _item.Quantity > 0);
    }

    public void OnHover()
    {
        if (!_controller) return;

        var itemData = InventoryManager.GetData(_item.Type);
        _controller.HoverItem(_discovered ? itemData.Description : itemData.UnknownDescription);
    }

    public void Discard()
    {
        if (_trunk) {
            InventoryManager.i.Inventory(InventoryType.CAR).Remove(_item.Type, 1);
        }
        else {
            InventoryManager.i.Inventory(InventoryType.PLAYER).Remove(_item.Type, 1);
        }
        Initialize(_item);
    }

    public void Take()
    {
        var carInv = InventoryManager.i.Inventory(InventoryType.CAR);
        var playerInv = InventoryManager.i.Inventory(InventoryType.PLAYER);

        if (carInv.GetCount(_item.Type) == 0) return;
        
        carInv.Remove(_item.Type, 1);
        playerInv.Add(_item.Type, 1);

        Initialize(_item);
    }

    public void TakeAll()
    {
        var carInv = InventoryManager.i.Inventory(InventoryType.CAR);
        var playerInv = InventoryManager.i.Inventory(InventoryType.PLAYER);

        var carCount = carInv.GetCount(_item.Type);

        if (carCount == 0) return;

        carInv.Remove(_item.Type, carCount);
        playerInv.Add(_item.Type, carCount);

        Initialize(_item);
    }

    public void Store()
    {
        var carInv = InventoryManager.i.Inventory(InventoryType.CAR);
        var playerInv = InventoryManager.i.Inventory(InventoryType.PLAYER);

        if (playerInv.GetCount(_item.Type) == 0) return;

        carInv.Add(_item.Type, 1);
        playerInv.Remove(_item.Type, 1);

        Initialize(_item);
    }

    public void StoreAll()
    {
        var carInv = InventoryManager.i.Inventory(InventoryType.CAR);
        var playerInv = InventoryManager.i.Inventory(InventoryType.PLAYER);

        var PlayerCount = playerInv.GetCount(_item.Type);

        if (PlayerCount == 0) return;

        playerInv.Remove(_item.Type, PlayerCount);
        carInv.Add(_item.Type, PlayerCount);

        Initialize(_item);
    }
}
