using MyBox;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private string _displayname;
    [SerializeField] private bool _breakable;
    [SerializeField, ConditionalField(nameof(_breakable))] private float _breakTime;

    public bool Breakable => _breakable;   
    public float BreakTime => _breakTime;
    public string DisplayName => _displayname;
}
