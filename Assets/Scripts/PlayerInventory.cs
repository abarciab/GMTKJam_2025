using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
    [SerializeField] private List<Item> _items;

    public List<Item> Items => new List<Item>(_items);

    public void OnValidate()
    {
        foreach (var item in _items) if (item.Data != null) item.Name = item.Data.DisplayName;
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

    public void Additems(ItemType itemType, int quantityToAdd = 1)
    {
        foreach (var item in _items) {
            if (item.Data.Type == itemType) {
                item.Quantity += quantityToAdd;
                return;
            }
        }
        
        AddNewItem(itemType, quantityToAdd);
    }

    public void Additems(Item newItem)
    {
        foreach (var item in _items) {
            if (item.Data.Type == newItem.Data.Type) {
                item.Quantity += newItem.Quantity;
                return;
            }
        }

        _items.Add(newItem);
    }

    private void AddNewItem(ItemType type, int quantity)
    {
        var allItems = GameManager.i.AllItems;
        var newItemData = allItems.Where(x => x.Type == type).First();
        var newItem = new Item(newItemData, quantity);
        _items.Add(newItem);
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

    public bool Contains(Inventory other)
    {
        var otherItems = other.Items;
        foreach (var i in otherItems) {
            if (GetCount(i.Data.Type) < i.Quantity) return false;
        }

        return true;
    }

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
    [SerializeField] private Inventory _inventory;

    public Inventory Inventory => _inventory;

    public int GetCount(ItemType Item) => _inventory.GetCount(Item);

    private void OnValidate()
    {
        _inventory.OnValidate();   
    }

    private void Start()
    {
        UpdateUI();
    }

    public void Additems(ItemType itemType, int quantityToAdd = 1)
    {
        _inventory.Additems(itemType, quantityToAdd);
        UpdateUI();
    }

    public void Additems(Item newItem)
    {
        _inventory.Additems(newItem);
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
        _inventory.RemoveItems(itemType, quantityToRemove);
        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.i.Do(UIAction.DISPLAY_INVENTORY, _inventory.Items);
    }
}
