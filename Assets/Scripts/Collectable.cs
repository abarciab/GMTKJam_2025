using MyBox;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private Animator _blockAnimator;
    [SerializeField] private string _displayname;
    [SerializeField] private Item _drop;
    [SerializeField] private bool _breakable;
    [SerializeField, ConditionalField(nameof(_breakable))] private float _breakTime;
    [SerializeField, ConditionalField(nameof(_breakable))] private Sound _breakSound;

    private float _idleTimer = 0.1f;
    private float _idleTimerLeft;

    public bool Breakable => _breakable;   
    public float BreakTime => _breakTime;
    public string DisplayName => _displayname;
    public Item Drop => _drop;

    private void Start()
    {
        if (_breakSound) _breakSound = Instantiate(_breakSound);
    }

    private void Update()
    {
        if (!_breakable) return;
        _idleTimerLeft -= Time.deltaTime;
        if (_idleTimerLeft <= 0) SetBreakProgress(0);
    }

    private void OnDisable()
    {
        if (_breakSound) _breakSound.Stop();
    }

    public void SetBreakProgress(float progress)
    {
        if (progress > 0) {
            _idleTimerLeft = _idleTimer;
            _breakSound.Play(restart: false);
        }

        _blockAnimator.SetBool("Breaking", progress > 0);
    }
}
