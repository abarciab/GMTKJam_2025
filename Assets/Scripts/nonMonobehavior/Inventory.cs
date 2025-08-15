using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable, InlineProperty]
public class Item
{
    [HideInInspector] public string Name;
    [HorizontalGroup(width: 0.4f, PaddingRight = 0.03f), HideLabel()] public ItemType Type;
    [HorizontalGroup(), MyBox.MinValue(0), HideLabel()] public int Quantity = 1;

    public Item(ItemType data, int quantity)
    {
        Type = data;
        Quantity = quantity;
    }

    public Item(Item original)
    {
        Type = original.Type;
        Quantity = original.Quantity;
    }
}

[System.Serializable]
public class ItemList
{
    [HideInInspector] public string Name;
    [LabelText("$Name")]
    public List<Item> Items;
}

[System.Serializable]
public class Inventory
{

    [HideInInspector] public string Name;
    public bool Player;
    [SerializeField, MyBox.MinValue(0), HorizontalGroup(PaddingRight = 0.1f)] private int _weightLimit = 15;
    [SerializeField, ProgressBar(0, 1, ColorGetter = "#d30f0f", DrawValueLabel = false), ReadOnly, HideLabel, HorizontalGroup(width:0.3f)] private float _percent;
    [SerializeField] private List<Item> _items = new List<Item>();

    public float WeightLimit => _weightLimit;
    public List<Item> Items => new List<Item>(_items);
    public float GetWeight() => _items.Sum(item => item.Quantity * InventoryManager.GetData(item.Type).Weight);
    public int GetCount(ItemType itemType) => Get(itemType).Quantity;
    public bool Contains(List<Item> items) => items.All(item => GetCount(item.Type) > item.Quantity);

    public void OnValidate()
    {
        foreach (var item in _items) item.Name = item.Type.ToString();
        _percent = GetWeight() / _weightLimit;
    }

    private void PlayerInventoryAdd(ItemType itemType)
    {
        if (!Player) return;
        var item = Get(itemType);
        InventoryManager.i.DiscoverItem(itemType);
        if (item.Quantity > 0) UIManager.i.Do(UIAction.SHOW_POPUP, "+ " + item.Quantity + " " + InventoryManager.i.GetDisplayName(itemType));
    }

    public Item Get(ItemType itemType)
    {
        var selected = _items.Where(x => x.Type == itemType);
        if (selected.Count() == 0) Add(itemType);
        return _items.Where(x => x.Type == itemType).FirstOrDefault();
    }

    public void Add(List<Item> items) => items.ForEach(item => Add(item.Type, item.Quantity));
    public void Add(ItemType itemType, int quantity = 1)
    {
        PlayerInventoryAdd(itemType);

        foreach (var item in _items.Where(x => x.Type == itemType)) {
            item.Quantity += quantity;
            return;
        }

        _items.Add(new Item(itemType, quantity));
    }

    public void Remove(List<Item> items) => items.ForEach(item => Remove(item.Type, item.Quantity));
    public void Remove(ItemType itemType, int quantityToRemove = 1)
    {
        foreach (var item in _items.Where(x => x.Type == itemType)) {
            item.Quantity -= quantityToRemove;
            item.Quantity = Mathf.Max(0, item.Quantity);
        }
    }
}