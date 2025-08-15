using MyBox;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;

    [Title("Timer")]
    [SerializeField] private float _totalTime;
    [SerializeField] private float _timePercentMod = 0.9f;

    [FoldoutGroup("References"), SerializeField] private Player _player;
    [FoldoutGroup("References"), SerializeField] private Car _car;
    [FoldoutGroup("References"), SerializeField] private CameraController _camera;
    [FoldoutGroup("References"), SerializeField] private EnvironmentController _environment;
    [FoldoutGroup("References"), SerializeField] private HarrowedSpawner _spawner;

    private float _timeLeft;
    private bool _timerPaused;

    public Player Player => _player;
    public Car Car => _car;
    public CameraController Camera => _camera;
    public void LoadMenu() => Utils.TransitionToScene(0);
    public void EndGame() => Utils.TransitionToScene(2);
    public void ResumeTimer() => _timerPaused = false;
    public void PauseTimer() => _timerPaused = true;

    private void Awake()
    {
        if (i != null) Destroy(i.gameObject);
        i = this;
    }

    private void Start()
    {
        Utils.SetCursor(false);
        _timeLeft = _totalTime;
    }

    private void Update()
    {
        if (_player.Dead) return;

        if (!_timerPaused) _timeLeft -= Time.deltaTime;
        var timerPercent = _timeLeft / _totalTime;
        UIManager.i.Do(UIAction.UPDATE_TIMER, timerPercent);

        _spawner.UpdatePercent(timerPercent);   
    }

    public void EnterNewArea()
    {
        _environment.EnterNewArea();
        _spawner.EnterNewArea();
        
        _totalTime *= _timePercentMod;
        _timeLeft = _totalTime;
    }

    public void CompleteArea()
    {
        UIManager.i.Do(UIAction.SHOW_TRANSITION);
        _environment.gameObject.SetActive(false);
    }

    public void LoseGame()
    {
        _player.Die();
        UIManager.i.Do(UIAction.SHOW_DEATH_SCREEN);
    }
}
