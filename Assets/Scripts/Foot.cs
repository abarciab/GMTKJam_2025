using UnityEngine;

[RequireComponent (typeof(Collider))]
public class Foot : MonoBehaviour
{
    [SerializeField] private Transform _shadowCaster;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private bool _rising = true;
    [SerializeField] private float _riseSpeed;
    [SerializeField] private float _fallSpeed;
    [SerializeField] private float _maxY;
    [SerializeField] private float _waitTime;
    [SerializeField] private LayerMask _floor;
    [SerializeField] private Sound _stepSound;
    [SerializeField] private float _forwardSpeed = 10;

    private float _groundY = 0;
    private Collider _collider;
    private Vector3 _shadowOriginalScale;
    private float _waitTimeLeft;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _stepSound = Instantiate(_stepSound);
        var didHit = Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out var hitInfo, 1000);
        if (didHit) _groundY = hitInfo.point.y;
        _shadowOriginalScale = _shadowCaster.localScale;
    }

    void Update()
    {
        if (_waitTimeLeft > 0) {
            _waitTimeLeft -= Time.deltaTime;
            return;
        }

        _collider.isTrigger = !_rising;
        var progressUp = (transform.position.y - _groundY) / (_maxY - _groundY);
        if (progressUp > 0.1f) transform.position += transform.forward * _forwardSpeed * Time.deltaTime;

        _shadowCaster.position = new Vector3(transform.position.x, _groundY + 1, transform.position.z);
        _shadowCaster.localScale = Vector3.Lerp(_shadowOriginalScale, Vector3.zero, progressUp);        

        if (_rising) Rise();
        else Fall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Car>()) {
            GameManager.i.LoseGame();
        }
    }

    private void Rise()
    {
        var progressUp = (transform.position.y - _groundY) / (_maxY - _groundY);
        var speed = _speedCurve.Evaluate(progressUp) * _riseSpeed;
        transform.position += Vector3.up * Mathf.Max(speed, 0.1f) * Time.deltaTime;
        if (transform.position.y > _maxY) _rising = false;
    }

    private void Fall()
    {
        var progressDown = 1 - ( (transform.position.y - _groundY) / (_maxY - _groundY));
        var speed = _speedCurve.Evaluate(progressDown) * _fallSpeed;
        transform.position += Vector3.down * Mathf.Max(speed, 0.1f) * Time.deltaTime;
        if (transform.position.y <= _groundY) {
            FindFirstObjectByType<CameraShake>().ShakeDefault();
            _stepSound.Play();
            _rising = true;
            _waitTimeLeft = _waitTime;
        }
    }
}
