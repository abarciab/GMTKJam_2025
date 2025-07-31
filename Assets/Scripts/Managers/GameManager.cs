using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;

    [SerializeField] private float _totalTime;
    [SerializeField] private Player _player;

    private float _timeLeft;

    public void LoadMenu() => Utils.TransitionToScene(0);
    public void EndGame() => Utils.TransitionToScene(2);
    public Transform Player => _player.transform;
    public Transform Camera => null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _timeLeft = _totalTime;
    }

    private void Awake()
    {
        if (i != null) Destroy(i.gameObject);
        i = this;
    }

    private void Update()
    {
        if (_player.Dead) return;

        _timeLeft -= Time.deltaTime;
        UIManager.i.Do(UIAction.UPDATE_TIMER, _timeLeft / _totalTime);

        if (_timeLeft <= 0) LoseGame();
    }

    private void LoseGame()
    {
        _player.Die();
        UIManager.i.Do(UIAction.SHOW_DEATH_SCREEN);
    }
}
