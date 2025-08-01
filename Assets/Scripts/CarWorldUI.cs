using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CarWorldUI : MonoBehaviour
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private RectTransform _pointerRTransform;
    public Transform EndGate;

    private void Update()
    {
        if (!EndGate) return;

        Vector3 toTarget = EndGate.position - _pointerRTransform.position;
        toTarget.y = 0; 
        float angle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        _pointerRTransform.rotation = Quaternion.Euler(-90f, 0f, angle);
    }
}
