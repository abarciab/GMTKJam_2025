using MyBox;
using UnityEngine;

[RequireComponent (typeof(Collider), typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private float _inertTime;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _floorlayer;

    private Collider _collider;
    private Rigidbody _rb;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        _inertTime -= Time.deltaTime;
        if (_inertTime <= 0) _collider.enabled = true;

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0) Destroy(gameObject);

        if (!_rb.isKinematic) transform.forward = _rb.linearVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>()) return;

        _collider.enabled = false;
        _rb.isKinematic = true;

        if (collision.gameObject.layer != _floorlayer) transform.parent = collision.gameObject.transform;
    }
}
