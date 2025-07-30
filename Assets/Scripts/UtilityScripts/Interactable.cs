using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent _onActivate;
    [SerializeField] private float _delayTime;
    [SerializeField] private bool _singleUse = true;
    [SerializeField] private GameObject _prompt;

    private bool _active;
    private bool _midActivation;
    private Transform _player;

    private void Start()
    {
        if (_prompt) _prompt.SetActive(false);
    }

    private void Update()
    {
        if (!_active) return;
        if (InputController.GetDown(Control.INTERACT)) Activate();
    }

    private void OnDisable()
    {
        if (_prompt) _prompt.SetActive(false);
    }

    private void Activate(bool waited = false)
    {
        if (_midActivation) return;

        if (_delayTime < 0.01f || waited == true) {
            _onActivate.Invoke();
            if (_singleUse) enabled = false;
        }
        else {
            StartCoroutine(WaitThenActivate());
        }
    }

    private IEnumerator WaitThenActivate()
    {
        _midActivation = true;
        yield return new WaitForSeconds(_delayTime);
        _midActivation = false;
        Activate(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.transform == _player) {
            _active = true;
            if (_prompt) _prompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        //if (other.GetComponent<Player>()) {
        if (true) {
            _active = false;
            if (_prompt) _prompt.SetActive(false);
        }
    }
}
