using MyBox;
using UnityEngine;

public class WalkCamera : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _lookTargetOffset;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Vector2 _lookLimits = new Vector2(-50, 80);

    private float _currentLook = 0;

    public Vector3 TargetPosition { get; private set; }
    public Quaternion TargetRotation { get; private set; }
    public Quaternion TargetCamLocalRotation { get; private set; }

    public void UpdatePositionAndRotation()
    {
        if (!_player.Frozen) {
            Turn();
        }

        TargetPosition = _player.transform.TransformPoint(_positionOffset);

        Vector3 targetPos = _player.transform.TransformPoint(_lookTargetOffset);
        Vector3 direction = targetPos - TargetPosition;
        TargetRotation = Quaternion.LookRotation(direction, Vector3.up);

    }
    private void Turn()
    {
        var mouseDelta = -Input.mousePositionDelta.y * Settings.MouseSensitivity.y;
        var rotDelta = mouseDelta * _rotateSpeed * Time.deltaTime * 100;
        if (rotDelta > 0) rotDelta = Mathf.Min(rotDelta, _lookLimits.y - _currentLook);
        if (rotDelta < 0) rotDelta = Mathf.Max(rotDelta, _lookLimits.x - _currentLook);
        _currentLook += rotDelta;

        var rot = Quaternion.Euler(_currentLook, 0, 0);
        TargetCamLocalRotation = rot;
    }
    public void SnapToPlayer()
    {
        if (!_player) _player = FindFirstObjectByType<Player>();
        transform.position = _player.transform.TransformPoint(_positionOffset);
        transform.LookAt(_player.transform.TransformPoint(_lookTargetOffset));
        Utils.SetDirty(transform);
    }
    

}
