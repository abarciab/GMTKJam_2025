using MyBox;
using UnityEngine;

[RequireComponent (typeof(Collider))]
[SelectionBase]
public class Foot : MonoBehaviour
{
    [SerializeField] private RockExplosion _explosion;
    [SerializeField] private Transform _shadowCaster;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private Vector2 _xRotLimits;
    [SerializeField] private bool _rising = true;
    [SerializeField] private float _shakeRadius;
    [SerializeField] private float _riseSpeed;
    [SerializeField] private float _fallSpeed;
    [SerializeField] private float _maxY;
    [SerializeField] private float _waitTime;
    [SerializeField] private LayerMask _floor;
    [SerializeField] private Sound _stepSound;
    [SerializeField] private Sound _stepSoundLocal;
    [SerializeField] private float _forwardSpeed = 10;
    public float InitialWaitTime = 5;

    private float _groundY = 0;
    private Collider _collider;
    private Vector3 _shadowOriginalScale;
    private float _waitTimeLeft;
    Quaternion _originalLocalRot;
    Quaternion _targetRot;

    Transform _player;

    private void Start()
    {
        _player = GameManager.i.Player.transform;

        _originalLocalRot = transform.localRotation;
        _collider = GetComponent<Collider>();
        _stepSound = Instantiate(_stepSound);
        _stepSoundLocal = Instantiate(_stepSoundLocal);
        var didHit = Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out var hitInfo, 1000);
        if (didHit) _groundY = hitInfo.point.y;
        _shadowOriginalScale = _shadowCaster.localScale;

        var y = _maxY;
        if (_rising) y = _groundY;
        var pos = transform.position;
        pos.y = y;
        transform.position = pos;

        if (_rising) _waitTimeLeft = _waitTime;
    }

    void Update()
    {
        var didHit = Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out var hitInfo, 1000);
        if (didHit) _groundY = hitInfo.point.y;

        _collider.isTrigger = !_rising;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRot, 2 * Time.deltaTime);

        if (InitialWaitTime <= 0 && _waitTimeLeft > 0) {
            _waitTimeLeft -= Time.deltaTime;

            _targetRot = _originalLocalRot;
            return;
        }

        var progressUp = (transform.position.y - _groundY) / (_maxY - _groundY);

        
        if (_rising) {
            var targetEuler = transform.localEulerAngles;
            targetEuler.x = _xRotLimits.x;
            _targetRot = Quaternion.Lerp(_originalLocalRot, Quaternion.Euler(targetEuler), 1 - progressUp);
        }
        else {
            var targetEuler = transform.localEulerAngles;
            targetEuler.x = _xRotLimits.y;
            _targetRot = Quaternion.Lerp(_originalLocalRot, Quaternion.Euler(targetEuler), progressUp);
        }

        if (progressUp > 0.1f) transform.position += transform.forward * _forwardSpeed * Time.deltaTime;

        if (InitialWaitTime > 0) {
            InitialWaitTime -= Time.deltaTime;
            return;
        }

        _shadowCaster.position = new Vector3(transform.position.x, _groundY + 2.5f, transform.position.z);
        _shadowCaster.localScale = Vector3.Lerp(_shadowOriginalScale, Vector3.zero, progressUp);        

        if (_rising) Rise();
        else Fall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Car>()){
            GameManager.i.LoseGame();
        }
    }

    private void OnDrawGizmosSelected()
    {
        var top = transform.position;
        top.y = _maxY;
        var bottom = transform.position;
        if (Application.isPlaying) bottom.y = _groundY;
        else bottom.y = 0;
        Gizmos.DrawLine(bottom, top);

        Gizmos.DrawWireSphere(transform.position, _shakeRadius);
    }

    private void Rise()
    {
        var progressUp = (transform.position.y - _groundY) / (_maxY - _groundY);
        var speed = _speedCurve.Evaluate(progressUp) * _riseSpeed;
        transform.position += Vector3.up * Mathf.Max(speed, 0.1f) * Time.deltaTime;
        if (transform.position.y > _maxY) {
            _rising = false;
        }
    }

    private void Fall()
    {
        var progressDown = 1 - ( (transform.position.y - _groundY) / (_maxY - _groundY));
        var speed = _speedCurve.Evaluate(progressDown) * _fallSpeed;
        transform.position += Vector3.down * Mathf.Max(speed, 0.1f) * Time.deltaTime;
        if (transform.position.y <= _groundY) {

            if (Vector3.Distance(_player.position, transform.position) < _shakeRadius) { 
                FindFirstObjectByType<CameraShake>().ShakeDefault();
            }

            _explosion.SpawnRocks();
            _stepSoundLocal.Play(transform);
            _rising = true;
            _waitTimeLeft = _waitTime;
        }
    }
}
