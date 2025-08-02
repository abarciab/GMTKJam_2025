using MyBox;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _inertTime;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _floorlayer;
    [SerializeField] private Sound _landSound;
    [SerializeField] private Sound _hitSound;
    [SerializeField] private Animator _arrowAnimator;
    [SerializeField] private Collider _collider;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _landSound = Instantiate(_landSound);
        _hitSound = Instantiate(_hitSound);
    }

    private void Update()
    {
        _inertTime -= Time.deltaTime;
        if (_inertTime <= 0 && _collider) _collider.enabled = true;

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0) Destroy(gameObject);

        if (_rb && _rb.linearVelocity != Vector3.zero) transform.forward = _rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>()) return;

        var animal = other.GetComponent<Animal>();
        if (animal) {
            animal.Damage();
            _hitSound.Play();
        }
        else _landSound.Play(transform);


        Destroy(this);
        Destroy(_rb);
        Destroy(_collider);
        _arrowAnimator.SetTrigger("Land");

        if (other.gameObject.layer != _floorlayer) transform.parent = other.gameObject.transform;
    }
}
