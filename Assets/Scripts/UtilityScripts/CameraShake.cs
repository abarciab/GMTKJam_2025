using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake: MonoBehaviour
{

    private Vector3 _targetPoint;
    private float _distanceThreshold = 0.05f;
    [SerializeField] private Transform _camera;

    [Header("Parameters")]
    [SerializeField] private Vector2 _speedRange = new Vector2(0.05f, 5);
    [SerializeField] private Vector2 _amplitudeRange = new Vector2(0.1f, 2);
    [SerializeField] private float _defaultspeed = 1;
    [SerializeField] private float _defaultAmplitude = 0.5f;
    [SerializeField] private float _defaultDuration = 0.1f;

    private float _speedMod = 10;
    private float _amplitudeMod = 0.1f;


    public void ShakeDefault() => CallShake(_defaultspeed, _defaultAmplitude, _defaultDuration);
    public void ShakeUnrestricted(float speed, float amplitude, float duration) => CallShake(speed, amplitude, duration);
    public void ShakeManual(float speedMod, float amplitudeMod, float duration)
    {
        speedMod = Mathf.Clamp01(speedMod);
        float speed = Mathf.Lerp(_speedRange.x, _speedRange.y, speedMod);
        amplitudeMod = Mathf.Clamp01(amplitudeMod);
        float amplitude = Mathf.Lerp(_amplitudeRange.x, _amplitudeRange.y, amplitudeMod);
        CallShake(speed, amplitude, duration);
    }

    private void CallShake(float speed, float amplitude, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeFixed(speed, amplitude, duration));
    }

    private IEnumerator ShakeFixed(float speed, float amplitude, float duration)
    {
        speed *= _speedMod;
        amplitude *= _amplitudeMod;


        SetNewTargetPoint(amplitude);

        float timePassed = 0;
        while (timePassed < duration) {
            MoveCameraTowardSelectedPoint(speed * Time.deltaTime);
            if (DistanceToPoint() < _distanceThreshold) SetNewTargetPoint(amplitude);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _targetPoint = Vector3.zero;
        _camera.localPosition = Vector3.zero;
    }

    private void MoveCameraTowardSelectedPoint(float amount)
    {
        var dir = (_targetPoint - _camera.localPosition).normalized;
        _camera.localPosition += dir * amount;
    }

    private void SetNewTargetPoint(float amplitude)
    {
        _targetPoint = Random.onUnitSphere * amplitude;
    }

    private float DistanceToPoint()
    {
        return Vector3.Distance(_camera.localPosition, _targetPoint);
    }
}
