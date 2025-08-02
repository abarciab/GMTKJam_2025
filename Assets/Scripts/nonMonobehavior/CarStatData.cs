using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarStatData", menuName = "Scriptable Objects/CarStatData")]
public class CarStatData : ScriptableObject
{
    public CarStatType Type;
    public string DisplayName;
    [TextArea(3, 10)] public string Description;
    [TextArea(3, 10)] public string AmountSuffix = "%";
    public float MaxValue;
    [SerializeField] private List<Inventory> _upgradeCosts = new List<Inventory>();

    public float LevelUpgradeAmount => MaxValue / _upgradeCosts.Count;

    public List<Item> CostForNextLevel(int currentLevel)
    {
        if (currentLevel == _upgradeCosts.Count) return new List<Item>();
        return _upgradeCosts[currentLevel].Items;
    }

    private void OnValidate()
    {
        for (int i = 0; i < _upgradeCosts.Count; i++) {
            _upgradeCosts[i].Name = "Level " + (i + 1);
        }
    }

}
