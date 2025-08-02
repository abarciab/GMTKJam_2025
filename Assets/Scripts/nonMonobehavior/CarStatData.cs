using UnityEngine;

[CreateAssetMenu(fileName = "CarStatData", menuName = "Scriptable Objects/CarStatData")]
public class CarStatData : ScriptableObject
{
    public CarStatType Type;
    public string DisplayName;
    public string Description;
    public float MaxValue;
}
