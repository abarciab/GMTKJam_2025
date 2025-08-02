using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _quantityText;
    [SerializeField] private GameObject _quantityParent;
    [SerializeField] private bool _redWhenunaffordable;
    [SerializeField] private SelectableItem _quantity;

    private PlayerInventoryUIController _controller;
    private Item _item;
    private bool _discovered;

    public void OnHover() => _controller.HoverItem(_discovered ? _item.Data.Description : _item.Data.UnknownDescription);
    public void EndHover() => _controller.EndHover();
    public void Initialize(Item item, PlayerInventoryUIController controller)
    {
        _controller = controller;
        Initialize(item);
    }

    public void Initialize(Item item)
    {
        _item = item;
        _icon.sprite = item.Data.Icon;

        _discovered = GameManager.i.ItemDiscovered(item.Data.Type);
        if (_discovered) _nameText.text = item.Data.DisplayName;
        else _nameText.text = "???";

        _quantityParent.SetActive(item.Quantity > 0);
        _quantityText.text = item.Quantity.ToString();

        if (_quantity) {
            var player = GameManager.i.Player;
            if (_redWhenunaffordable && player.GetComponent<PlayerInventory>().GetCount(item.Data.Type) < item.Quantity) {
                _quantity.SetDisabled(true);
            }
            else {
                _quantity.SetDisabled(false);
            }
        }
    }
}
