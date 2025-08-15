using Sirenix.OdinInspector;
using UnityEngine;

public enum ItemType { FADE_STONE, HOLLOW_PINE, SAP, FLINT, ASH, LANTERN_SPORE, SOUL_SHARD, SOUL, SCRAP, WIRE, CLOTH, GLASS}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [HorizontalGroup()] public string DisplayName;
    [HorizontalGroup()] public ItemType Type;
    [TextArea(3, 10)] public string Description;
    [TextArea(3, 10)] public string UnknownDescription;
    [MinValue(0)] public float Weight;
    public Sprite Icon;
}
