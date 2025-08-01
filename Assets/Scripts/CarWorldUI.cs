using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CarWorldUI : MonoBehaviour
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private RectTransform _pointerRTransform;
    [SerializeField] private Transform _goal;

    private void Update()
    {
        // Get direction to target in the XZ plane
        Vector3 toTarget = _goal.position - _pointerRTransform.position;
        toTarget.y = 0; // flatten to horizontal plane

        // Get the angle between forward and the target
        float angle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;

        // Apply rotation: lock X and Y, rotate only around Z
        _pointerRTransform.rotation = Quaternion.Euler(-90f, 0f, angle);
    }
}
