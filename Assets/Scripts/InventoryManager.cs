using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InventoryType { PLAYER, CAR, COMBINED }

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i;

    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private Inventory _carInventory;

    [HideInInspector] private List<ItemData> _allItems = new List<ItemData>();
    
    private List<ItemType> _discoveredItems = new List<ItemType>();

    public float WeightLimit => _playerInventory.WeightLimit;
    public bool Encumbered => _playerInventory.GetWeight() > WeightLimit;
    public string GetDisplayName(ItemType type) => AllItems().Where(x => x.Type == type).First().DisplayName;
    public bool ItemDiscovered(ItemType type) => _discoveredItems.Contains(type);
    public int GetCount(ItemType Item) => _playerInventory.GetCount(Item);
    public static ItemData GetData(ItemType type) => (i ? i.AllItems() : new List<ItemData>(Resources.LoadAll<ItemData>("ItemData"))).Where(x => x.Type == type).First();   

    private void Awake()
    {
        i = this;
    }

    private void OnValidate()
    {
        _playerInventory.OnValidate();
        _carInventory.OnValidate();
    }

    private void Update()
    {
        if (Encumbered) UIManager.i.Do(UIAction.SHOW_STATUS, Status.ENCUMBERED);
        else UIManager.i.Do(UIAction.HIDE_STATUS, Status.ENCUMBERED);
    }

    public List<ItemData> AllItems()
    {
        if (_allItems.Count == 0) _allItems = new List<ItemData>(Resources.LoadAll<ItemData>("ItemData"));
        return _allItems;
    }

    public void DiscoverItem(ItemType type)
    {
        if (!_discoveredItems.Contains(type)) {
            var itemName = GetDisplayName(type);
            UIManager.i.Do(UIAction.SHOW_POPUP, "discovered " + itemName);
            _discoveredItems.Add(type);
        }
    }

    public Inventory Inventory(InventoryType type)
    {
        if (type == InventoryType.PLAYER) return _playerInventory;
        else if (type == InventoryType.CAR) return _carInventory;

        var combined = new Inventory();
        var items = new List<Item>();
        foreach (var item in _carInventory.Items) items.Add(new Item(item));
        foreach (var item in _playerInventory.Items) items.Add(new Item(item));

        combined.Add(items);
        return combined;
    }

    public void RemoveCombinedItems(List<Item> items)
    {
        foreach (var i in items) {
            var toRemove = i.Quantity;
            var playerCount = _playerInventory.GetCount(i.Type);
            if (playerCount > 0) {
                _playerInventory.Remove(i.Type, i.Quantity);
                toRemove -= Mathf.Min(playerCount, toRemove);
            }
            var carCount = _carInventory.GetCount(i.Type);
            if (carCount > 0) {
                _carInventory.Remove(i.Type, i.Quantity);
                toRemove -= Mathf.Min(carCount, toRemove);
            }
        }
    }

    public void RemoveItems(ItemType itemType, int quantityToRemove = 1)
    {
        _playerInventory.Remove(itemType, quantityToRemove);
    }
}
