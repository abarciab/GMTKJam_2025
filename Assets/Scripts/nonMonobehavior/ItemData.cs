using UnityEngine;

public enum ItemType { FADE_STONE, HOLLOW_PINE, SAP, FLINT, ASH, LANTERN_SPORE, SOUL_SHARD, SOUL, SCRAP, WIRE, CLOTH}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType Type;
    public string DisplayName;
}
