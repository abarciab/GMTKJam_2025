using UnityEngine;
using TMPro;
using MyBox;
using Unity.VisualScripting;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnimateText : MonoBehaviour
{
    [SerializeField] private bool _insertNewlines;
    [SerializeField, ConditionalField(nameof(_insertNewlines))] private int _newlineDist;
    [SerializeField] private float _letterDelaySpeed = 0.1f;
    [SerializeField] private Sound _letterSound;
    [SerializeField] private bool _animateStaticOnEnable;
    [HideInInspector] public UnityEvent OnAnimateComplete;

    private TextMeshProUGUI _text;
    private string _targetString;
    private float _cooldown;
    private int _index;
    private bool _setup;

    private void OnEnable()
    {
        Setup();
        if (_animateStaticOnEnable) Animate(_text.text);
    }

    private void Update()
    {
        if (_targetString == null || _index >= _targetString.Length) return;

        _cooldown -= Time.deltaTime;
        if (_cooldown < 0) TypeLetter();
    }

    private void Setup()
    {
        if (_setup) return;
        if (_letterSound) _letterSound = Instantiate(_letterSound);
        _text = GetComponent<TextMeshProUGUI>();
        _setup = true;
    }

    public void Animate(string text)
    {
        Setup();
        _targetString = text;
        if (_insertNewlines) InsertNewLines(); 

        _text.text = "";
        _index = 0;
    }

    private void InsertNewLines()
    {
        int count = 0;
        int _previousSpaceIndex = 0;
        for (int i = 0; i < _targetString.Length; i++) {
            count++;
            if (_targetString[i] == ' ') _previousSpaceIndex = i;
            if (count == _newlineDist) {
                _targetString = _targetString.Insert(_previousSpaceIndex, "\n");
                _targetString = _targetString.Remove(_previousSpaceIndex + 1, 1);
                count = 0;
            }
        }
    }

    private void TypeLetter()
    {
        _cooldown = _letterDelaySpeed;
        _text.text += _targetString[_index];
        _index += 1;
        if (_letterSound) _letterSound.Play(restart: false);
        if (_index >= _targetString.Length) OnAnimateComplete.Invoke();
    }
}
