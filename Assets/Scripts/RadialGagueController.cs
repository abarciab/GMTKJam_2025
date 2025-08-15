using MyBox;
using UnityEngine;

public class RadialGagueController : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    [SerializeField] private Vector2 _limits;
    [SerializeField] private Vector3 _axis = Vector3.forward;
    [SerializeField] private float _lerpFactor = 10;

    private float _targetValue;
    private float _currentValue;

    public void SetValue(float value) => _targetValue = value;

    private void Update()
    {
        _currentValue = Mathf.Lerp(_currentValue, _targetValue, _lerpFactor * Time.deltaTime);

        _hand.transform.localEulerAngles = _axis * Mathf.Lerp(_limits.x, _limits.y, _currentValue);
    }
}
