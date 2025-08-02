using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [SerializeField] private float _inertTime = 0.25f;
    [SerializeField] private float _lifeTime = 10;

    private bool _active = true;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.angularVelocity = Random.insideUnitSphere;
        _lifeTime *= Random.Range(0.7f, 1.3f);
    }

    private void Update()
    {
        _inertTime -= Time.deltaTime;

        _lifeTime -= Time.deltaTime;
        if (_lifeTime<= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_rb.linearVelocity.magnitude > 0.5f && (other.GetComponent<Player>() || other.GetComponent<Car>())) GameManager.i.LoseGame();

        if (_inertTime > 0 && _active || other.gameObject.layer != 3) return;

        _active = false;
        _rb.isKinematic = true;
    }

}
