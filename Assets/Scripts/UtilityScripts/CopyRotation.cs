using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    [SerializeField] private Transform _leader;
    [SerializeField] private Transform _follower;
    void Update()
    {
        _follower.rotation = _leader.rotation;
    }
}
