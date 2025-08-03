using MyBox;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public enum InventoryType { PLAYER, CAR, COMBINED}

[System.Serializable]
public class Item
{
    [HideInInspector] public string Name;
    [DisplayInspector] public ItemData Data;
    public int Quantity;

    public Item(ItemData data, int quantity)
    {
        Data = data;
        Quantity = quantity;
    }

    public Item(Item original)
    {
        Data = original.Data;
        Quantity = original.Quantity;
    }
}

[System.Serializable]
public class Inventory
{
    [HideInInspector] public string Name;
    [SerializeField] private List<Item> _items = new List<Item>();
    public bool Player;

    public List<Item> Items => new List<Item>(_items);

    public void OnValidate()
    {
        foreach (var item in _items) if (item.Data != null) item.Name = item.Data.DisplayName;
    }

    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var item in _items) total += item.Quantity;
        return total;
    }

    private void PlayerInventoryAdd(Item item)
    {
        if (!Player) return;
        PlayerInventoryAdd(item.Data.Type, item.Quantity);
    }

    private void PlayerInventoryAdd(ItemType itemType, int quantity)
    {
        if (!Player) return;
        if (quantity > 0) {
            GameManager.i.DiscoverItem(itemType);
            UIManager.i.Do(UIAction.SHOW_POPUP, "+ " +  quantity + " " + GameManager.i.GetDisplayName(itemType));
        }
    }

    public Item getItem(ItemType itemType)
    {
        foreach (var item in _items) {
            if (item.Data.Type == itemType) {
                return item;
            }
        }

        return AddNewItem(itemType, 0);
    }

    public int GetCount(ItemType itemType)
    {
        foreach (var item in _items) {
            if (item.Data.Type == itemType) {
                return item.Quantity;
            }
        }

        AddNewItem(itemType, 0);
        return 0;
    }

    public void AddItems(List<Item> items)
    {
        foreach (var item in items) {
            AddItems(item);
        }
    }

    public void AddItems(ItemType itemType, int quantityToAdd = 1)
    {
        foreach (var item in _items) {
            if (item.Data.Type == itemType) {
                item.Quantity += quantityToAdd;
                PlayerInventoryAdd(itemType, quantityToAdd);
                return;
            }
        }

        AddNewItem(itemType, quantityToAdd);
    }

    public void AddItems(Item newItem)
    {
        foreach (var item in _items) {
            if (item.Data.Type == newItem.Data.Type) {
                item.Quantity += newItem.Quantity;
                PlayerInventoryAdd(newItem);
                return;
            }
        }

        PlayerInventoryAdd(newItem);

        _items.Add(newItem);
    }

    private Item AddNewItem(ItemType itemType, int quantity)
    {
        var allItems = GameManager.i.AllItems;
        var newItemData = allItems.Where(x => x.Type == itemType).First();
        var newItem = new Item(newItemData, quantity);
        _items.Add(newItem);

        PlayerInventoryAdd(newItem);

        return newItem;
    }

    public void Subtract(Inventory other)
    {
        foreach (var item in other.Items) {
            RemoveItems(item);
        }
    }

    public void RemoveItems(List<Item> items)
    {
        foreach (var item in items) {
            RemoveItems(item);
        }
    }

    public void RemoveItems(ItemType itemType, int quantityToRemove = 1)
    {
        foreach (var item in _items) {
            if (item.Data.Type == itemType) {
                item.Quantity -= quantityToRemove;
                item.Quantity = Mathf.Max(0, item.Quantity);
            }
        }
    }

    public void RemoveItems(Item item) => RemoveItems(item.Data.Type, item.Quantity);

    public bool Contains(List<Item> items)
    {
        foreach (var i in items) {
            if (GetCount(i.Data.Type) < i.Quantity) return false;
        }
        return true;
    }
    public bool Contains(Inventory other) => Contains(other.Items);


    /// <summary>
    /// Returns the items in the other inventory that aren't in this inventory, and how many
    /// </summary>
    /// <returns></returns>
    public List<Item> GetDifference(List<Item> otherItems)
    {
        var missing = new List<Item>();
        foreach (var item in otherItems) {
            var myCount = GetCount(item.Data.Type);
            if (myCount < item.Quantity) {
                missing.Add(new Item(item));
                missing[^1].Quantity = item.Quantity - myCount;
            }
        }

        return missing;
    }
    public List<Item> GetDifference(Inventory other)
    {
        return GetDifference(other.Items);
    }

    public List<Item> GetOverlap(List<Item> otherItems)
    {
        var overlap = new List<Item>();
        foreach (var item in otherItems) {
            var amount = Mathf.Min(item.Quantity, GetCount(item.Data.Type));
            if (amount > 0) {
                overlap.Add(new Item(item));
                overlap[^1].Quantity = amount;
            }
        }

        return overlap;
    }

    public List<Item> GetOverlap(Inventory other)
    {
        return GetOverlap(other.Items);
    }
}

public class PlayerInventory : MonoBehaviour
{
    public int _weightLimit = 15;
    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private Inventory _carInventory;

    public Inventory Inventory(InventoryType type)
    {
        if (type == InventoryType.PLAYER) return _playerInventory;
        else if (type == InventoryType.CAR) return _carInventory;

        var combined = new Inventory();
        var items = new List<Item>();
        foreach (var item in _carInventory.Items) items.Add(new Item(item));
        foreach (var item in _playerInventory.Items) items.Add(new Item(item));

        combined.AddItems(items);
        return combined;
    }

    public void RemoveCombinedItems(List<Item> items)
    {
        foreach (var i in items) {
            var playerCount = _playerInventory.GetCount(i.Data.Type);
            if (playerCount > 0) {
                _playerInventory.RemoveItems(i);
                i.Quantity -= playerCount;
            }
            var carCount = _carInventory.GetCount(i.Data.Type);
            if (carCount > 0) {
                _carInventory.RemoveItems(i);
                i.Quantity -= carCount;
            }
        }
    }

    public bool Encumbered => _playerInventory.GetTotalItemCount() > _weightLimit;

    public int GetCount(ItemType Item) => _playerInventory.GetCount(Item);

    private void OnValidate()
    {
        _playerInventory.OnValidate();   
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Encumbered) UIManager.i.Do(UIAction.SHOW_STATUS, Status.ENCUMBERED);
        else UIManager.i.Do(UIAction.HIDE_STATUS, Status.ENCUMBERED);
    }

    public void Additems(ItemType itemType, int quantityToAdd = 1)
    {
        _playerInventory.AddItems(itemType, quantityToAdd);
        UpdateUI();
    }

    public void Additems(Item newItem)
    {
        _playerInventory.AddItems(newItem);
        UpdateUI();
    }

    public void RemoveItems(List<Item> itemsToRemove)
    {
        foreach (var item in itemsToRemove) {
            RemoveItems(item.Data.Type, item.Quantity);
        }
        UpdateUI();
    }

    public void RemoveItems(ItemType itemType, int quantityToRemove = 1)
    {
        _playerInventory.RemoveItems(itemType, quantityToRemove);
        UpdateUI();
    }

    private void UpdateUI()
    {
        //UIManager.i.Do(UIAction.DISPLAY_INVENTORY, _inventory.Items);
    }
}
