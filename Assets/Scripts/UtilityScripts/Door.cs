using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DoorState
{
    public Vector3 Pos;
    public Quaternion Rot;

    public DoorState(Vector3 pos, Quaternion rot)
    {
        Pos = pos;
        Rot = rot;
    }
}

public class Door : MonoBehaviour
{

    [Header("Animation")]
    [SerializeField] private float _animateTime = 1.2f;
    [SerializeField] private AnimationCurve _curve;

    [Header("Automatic Behavior")]
    [SerializeField] private bool _openAutomatically;
    [SerializeField, ConditionalField(nameof(_openAutomatically))] private float _automaticDetectionRange = 3;

    [Header("Misc")]
    [SerializeField] private UnityEvent _onFinishAnimate;
    [SerializeField] private UnityEvent _onFinishOpen;
    [SerializeField] private UnityEvent _onFinishClose;
    [SerializeField] private Sound _openSound;

    [SerializeField, HideInInspector] private DoorState _openState;
    [SerializeField, HideInInspector] private DoorState _closeState;
    [SerializeField] private bool _open;

    private Transform _player;

    public bool IsOpen => _open;

    [ButtonMethod]
    private void SetClosed()
    {
        _closeState.Pos = transform.localPosition;
        _closeState.Rot = transform.localRotation;
        Utils.SetDirty(this);
    }

    [ButtonMethod]
    private void SetOpen()
    {
        _openState.Pos = transform.localPosition;
        _openState.Rot = transform.localRotation;
        Utils.SetDirty(this);
    }

    [ButtonMethod]
    public void Open()
    {
        _open = true;
        StopAllCoroutines();
        if (!Application.isPlaying) SnapToState(_openState, true);
        else StartCoroutine(AnimateToState(_openState, true));
    }

    [ButtonMethod]
    public void Close()
    {
        StopAllCoroutines();
        if (!Application.isPlaying) SnapToState(_closeState, false);
        else StartCoroutine(AnimateToState(_closeState, false));
    }

    private void Start()
    {
        _player = GameManager.i.Player.transform;
        if (_openSound) _openSound = Instantiate(_openSound);
    }

    private void Update()
    {
        if (!_openAutomatically) return;
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > _automaticDetectionRange * 1.5f && _open) Close();
        else if (dist < _automaticDetectionRange && !_open) Open();
    }

    private void SnapToState(DoorState state, bool openState)
    {
        transform.localPosition = state.Pos; 
        transform.localRotation = state.Rot;
        _open = openState;
    }

    private IEnumerator AnimateToState(DoorState target, bool targetStateOpen)
    {
        DoorState startState = new DoorState(transform.localPosition, transform.localRotation);

        if (_openSound && targetStateOpen) _openSound.Play();

        float timePassed = 0;
        while (timePassed <  _animateTime) {
            float progress = timePassed / _animateTime;
            progress = _curve.Evaluate(progress);

            transform.localPosition = Vector3.Lerp(startState.Pos, target.Pos, progress);
            transform.localRotation = Quaternion.Lerp(startState.Rot, target.Rot, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SnapToState(target, targetStateOpen);
        if (targetStateOpen) _onFinishOpen.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (_openAutomatically) Gizmos.DrawWireSphere(transform.position, _automaticDetectionRange);
    }
}
