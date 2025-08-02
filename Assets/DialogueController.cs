using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : UIController
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private float _letterTypeTime;
    [SerializeField] private Sound _letterTypeSound;

    private List<string> _lines;
    private int _currentLineIndex = 0;
    private int _currentLetterIndex;
    private float _currentLetterCooldown;
    
    private string _currentLine => _lines[_currentLineIndex];

    private void OnEnable()
    {
        Utils.SetCursor(true);
    }

    private void Start()
    {
        _letterTypeSound = Instantiate(_letterTypeSound);
    }

    private void OnDisable()
    {
        if (GameManager.i) GameManager.i.ResumeTimer();
        Utils.SetCursor(false);
    }

    private void Update()
    {
        if (_currentLetterIndex >= _currentLine.Length) return;

        _currentLetterCooldown -= Time.deltaTime;
        if (_currentLetterCooldown <= 0) {
            if (_currentLine[_currentLetterIndex] != ' ') _letterTypeSound.Play();

            _currentLetterCooldown = _letterTypeTime;
            _mainText.text += _currentLine[_currentLetterIndex];
            _currentLetterIndex += 1;
        }
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.START_CONVERSATION && arg is List<string> lines) StartConversation(lines);
    }

    public void StartConversation(List<string> lines)
    {
        _lines = lines;
        if (GameManager.i) GameManager.i.PauseTimer();
        gameObject.SetActive(true);

        _currentLineIndex = -1;
        ShowNext();
    }

    public void ShowNext()
    {
        if (_currentLineIndex >= 0 && _currentLetterIndex < _currentLine.Length) {
            _mainText.text = _currentLine;
            _currentLetterIndex = _currentLine.Length;
            return;
        }

        _currentLineIndex += 1;
        if (_lines.Count > _currentLineIndex) {
            _mainText.text = "";
            _currentLetterIndex = 0;
        }
        else {
            EndConversation();
        }            
    }

    private void EndConversation()
    {
        gameObject.SetActive(false);

        if (!GameManager.i) return;
        FindFirstObjectByType<CameraController>().EndConversation();
        FindFirstObjectByType<Player>().EndConversation();
    }

    
}
