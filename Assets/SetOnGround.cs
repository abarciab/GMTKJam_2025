using UnityEngine;

public class SetOnGround : MonoBehaviour
{
    [SerializeField] private LayerMask _floor;

    private void Start()
    {
        var pos = transform.position;
        var didHit = Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out var hitInfo, 1000, _floor);
        if (didHit) pos.y = hitInfo.point.y;
        transform.position = pos;
    }
}
