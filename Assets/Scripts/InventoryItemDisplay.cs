using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private SelectableItem _quantity;
    [SerializeField] private GameObject _discardButton;
    [SerializeField] private GameObject _iconFrameParent;
    [SerializeField] private GameObject _trunkButtonParent;

    private PlayerInventoryUIController _controller;
    private Item _item;
    private bool _discovered;

    public void OnHover() => _controller?.HoverItem(_discovered ? _item.Data.Description : _item.Data.UnknownDescription);
    public void EndHover() => _controller?.EndHover();
    public void Initialize(Item item, PlayerInventoryUIController controller)
    {
        _controller = controller;
        Initialize(item);
    }

    public void Initialize(Item item)
    {
        gameObject.SetActive(true);

        _item = item;
        _icon.sprite = item.Data.Icon;

        _discovered = GameManager.i.ItemDiscovered(item.Data.Type) || _showUnknown;
        if (_discovered) _nameText.text = item.Data.DisplayName;
        else _nameText.text = "???";

        if (_trunkButtonParent) {
            _trunkButtonParent.SetActive(_trunk && _discovered);
            _iconFrameParent.SetActive(!_trunkButtonParent.activeSelf);
        }

        _quantityParent.SetActive(_discovered);
        _quantityText.text = item.Quantity.ToString();

        if (_quantity) {
            var player = GameManager.i.Player;
            if (_redWhenUnaffordable && player.GetComponent<PlayerInventory>().GetCount(item.Data.Type) < item.Quantity) {
                _quantity.SetDisabled(true);
            }
            else {
                _quantity.SetDisabled(false);
            }
        }

        if (_discardButton) _discardButton.SetActive(_allowDiscard && _item.Quantity > 0);
    }

    public void Discard()
    {
        if (_trunk) {
            GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.CAR).RemoveItems(_item.Data.Type, 1);
        }
        else {
            GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER).RemoveItems(_item.Data.Type, 1);
        }
        //_item.Quantity -= 1;
        Initialize(_item);
    }

    public void Take()
    {
        var carInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.CAR);
        var playerInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER);

        if (carInv.GetCount(_item.Data.Type) == 0) return;
        
        carInv.RemoveItems(_item.Data.Type, 1);
        playerInv.AddItems(_item.Data.Type, 1);

        Initialize(_item);
    }

    public void TakeAll()
    {
        var carInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.CAR);
        var playerInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER);

        var carCount = carInv.GetCount(_item.Data.Type);

        if (carCount == 0) return;

        carInv.RemoveItems(_item.Data.Type, carCount);
        playerInv.AddItems(_item.Data.Type, carCount);

        Initialize(_item);
    }

    public void Store()
    {
        var carInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.CAR);
        var playerInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER);

        if (playerInv.GetCount(_item.Data.Type) == 0) return;

        carInv.AddItems(_item.Data.Type, 1);
        playerInv.RemoveItems(_item.Data.Type, 1);

        Initialize(_item);
    }

    public void StoreAll()
    {
        var carInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.CAR);
        var playerInv = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER);

        var PlayerCount = playerInv.GetCount(_item.Data.Type);
        //print("storing all " + _item.Data.Type + ". player count: " + PlayerCount);

        if (PlayerCount == 0) return;

        playerInv.RemoveItems(_item.Data.Type, PlayerCount);
        carInv.AddItems(_item.Data.Type, PlayerCount);

        Initialize(_item);
    }
}
