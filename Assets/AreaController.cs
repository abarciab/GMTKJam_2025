using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField] private Transform _entrance;
    [SerializeField] private Transform _endGate;

    public void InitializeArea(Car car)
    {
        car.SetEndGate(_endGate);
    }

    public void EnterArea(Car car, CameraController camera)
    {
        car.transform.SetPositionAndRotation(_entrance.position, _entrance.rotation);
        
        car.SetFuel(3);
        car.SetThrottle(3);
        car.setWheelAngle(0);
        InitializeArea(car);

        camera.SnapToCar();

    }
}
