using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Normal Rotuine")]
    [SerializeField] private Vector2 _moveTimeRange = new Vector2();
    [SerializeField] private Vector2 _waitTimeRange = new Vector2();
    [SerializeField] private Vector2 _directionChangeTimeRange = new Vector2();
    [SerializeField] private int _numDirectionAttemps = 4;

    [Header("Misc")]
    [SerializeField] private GameObject _corpsePrefab;
    [SerializeField] private int _hp = 1;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _lerpFactor = 2;
    [SerializeField] private float _accelFactor = 2;
    [SerializeField] private LayerMask _groundLayer;

    [Header("scared")]
    [SerializeField] private float _scareTimeMax;

    private Vector3 _moveDelta;
    private float _waitCooldown;
    private float _moveCooldown;
    private float _directionChangeCooldown;

    private bool _scared;
    private float _timeScared;
    private Transform _player;
    private Vector3 _targetDirection;
    private bool _moving;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
        _targetDirection = Random.insideUnitSphere;
    }

    private void Update()
    {
        Move();

        if (_scared) {
            RunFromPlayer();
            _timeScared += Time.deltaTime;
            if (_timeScared > _scareTimeMax) {
                _scared = false;
            }
        }
        else DoNormalBehavior();
    }

    private void DoNormalBehavior()
    {
        _directionChangeCooldown -= Time.deltaTime;
        if (_directionChangeCooldown <= 0) {
            _directionChangeCooldown = Random.Range(_directionChangeTimeRange.x, _directionChangeTimeRange.y);
            ChangeDirection();
        }

        if (_moving) {
            _moveCooldown -= Time.deltaTime;

            if (_moveCooldown <= 0) {
                _waitCooldown = Random.Range(_waitTimeRange.x, _waitTimeRange.y);
                _moving = false;
            }
        }
        else {
            _waitCooldown -= Time.deltaTime;
            if (_waitCooldown <= 0) {
                _moveCooldown = Random.Range(_moveTimeRange.x, _moveTimeRange.y);
                _moving = true;
            }
        }
    }

    private void RunFromPlayer()
    {
        _targetDirection = transform.position - _player.position;
    }

    private void Move()
    {
        transform.position += _moveDelta;

        if (!_moving && !_scared) {
            _moveDelta = Vector3.Lerp(_moveDelta, Vector3.zero, _accelFactor * Time.deltaTime);
            return;
        }

        var hitGound = Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out var hitInfo, 1000, _groundLayer);
        if (!hitGound) return;
        transform.position = hitInfo.point;
        _targetDirection.y = 0;

        var rot = transform.localEulerAngles;
        transform.LookAt(transform.position + _targetDirection * 10);
        rot.x = transform.localEulerAngles.x;
        rot.z = transform.localEulerAngles.z;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rot), _lerpFactor * Time.deltaTime);

        _moveDelta = Vector3.Lerp(_moveDelta, transform.forward * _moveSpeed * Time.deltaTime, _accelFactor * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        var options = new List<Vector3>();
        for (int i = 0; i < _numDirectionAttemps; i++) {
            options.Add(Random.insideUnitSphere.normalized);
        }

        _targetDirection = options.OrderBy(x => Vector3.Distance(x, _targetDirection)).First();
    }


    public void Damage()
    {
        _scared = true;
        _timeScared = 0;
        _hp -= 1;
        if (_hp <= 0) Die();
    }

    private void Die()
    {
        Instantiate(_corpsePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
