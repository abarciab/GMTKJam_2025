using UnityEngine;

public enum CarPartType { DOOR, FUEL, BED}

public class CarPart : MonoBehaviour
{
    [SerializeField] CarPartType _part;

    private Car _car;


    public CarPartType Part => _part;
    public Car Car => _car;

    private void Start()
    {
        _car = GetComponentInParent<Car>();
    }
}
