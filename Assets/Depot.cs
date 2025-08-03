using UnityEngine;

public enum DepotType { FUEL, WRECK, DEBRIS, ROCKS, MISC}

[SelectionBase]
public class Depot : MonoBehaviour
{
    [SerializeField] private DepotType _type;

    private void OnValidate()
    {
        gameObject.name = _type.ToString().ToLower() + " depot";
    }
}
