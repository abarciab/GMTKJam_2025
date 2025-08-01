using MyBox;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _inertTime;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _floorlayer;
    [SerializeField] private Sound _landSound;
    [SerializeField] private Animator _arrowAnimator;

    private Collider _collider;
    private Rigidbody _rb;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _landSound = Instantiate(_landSound);
    }

    private void Update()
    {
        _inertTime -= Time.deltaTime;
        if (_inertTime <= 0 && _collider) _collider.enabled = true;

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0) Destroy(gameObject);

        if (_rb) transform.forward = _rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>()) return;

        _landSound.Play(transform);
        Destroy(this);
        Destroy(_rb);
        Destroy(_collider);
        _arrowAnimator.SetTrigger("Land");

        if (other.gameObject.layer != _floorlayer) transform.parent = other.gameObject.transform;
    }
}
