using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "CarStatData", menuName = "Scriptable Objects/CarStatData")]
public class CarStatData : ScriptableObject
{
    [HorizontalGroup()] public string DisplayName;
    [HorizontalGroup(width: 0.3f), HideLabel()] public CarStatType Type;
    [TextArea(3, 10)] public string Description;
    public float MaxValue;
    [SerializeField, MyBox.MinValue(0)] private int _maxLevel = 5;

    [Space(10)]
    [ListDrawerSettings(ShowFoldout = false, HideAddButton = true, HideRemoveButton = true, ShowItemCount = false)]
    [SerializeField] private List<ItemList> _upgradeCosts = new List<ItemList>();

    public float LevelUpgradeAmount => MaxValue / _maxLevel;

    private void OnValidate()
    {
        for (int i = 0; i < _maxLevel; i++) {
            if (i >= _upgradeCosts.Count) _upgradeCosts.Add(new ItemList());
            _upgradeCosts[i].Name = "level " + (i + 1);
            foreach (var item in _upgradeCosts[i].Items) item.Quantity = Mathf.Max(1, item.Quantity);
        }

        while (_upgradeCosts.Count > _maxLevel) _upgradeCosts.RemoveAt(_upgradeCosts.Count - 1);
    }

    public List<Item> CostForNextLevel(int currentLevel)
    {
        if (currentLevel == _upgradeCosts.Count) return new List<Item>();
        var items = new List<Item>();
        foreach (var item in _upgradeCosts[currentLevel].Items) {
            items.Add(new Item(item));
        }
        return items;
    }
}
