using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarStatData", menuName = "Scriptable Objects/CarStatData")]
public class CarStatData : ScriptableObject
{
    public CarStatType Type;
    public string DisplayName;
    [TextArea(3, 10)] public string Description;
    public float MaxValue;
    public string AmountSuffix = "%";
    [SerializeField] private List<Inventory> _upgradeCosts = new List<Inventory>();

    public float LevelUpgradeAmount => MaxValue / _upgradeCosts.Count;

    public List<Item> CostForNextLevel(int currentLevel)
    {
        if (currentLevel == _upgradeCosts.Count) return new List<Item>();
        var items = new List<Item>();
        foreach (var item in _upgradeCosts[currentLevel].Items) {
            items.Add(new Item(item));
        }
        return items;
    }

    private void OnValidate()
    {
        for (int i = 0; i < _upgradeCosts.Count; i++) {
            _upgradeCosts[i].Name = "Level " + (i + 1);
        }
    }

}
