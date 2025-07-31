using UnityEngine;

public enum ItemType { ROCK, WOOD }

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType Type;
    public string DisplayName;
}
