using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;

    [SerializeField] private float _totalTime;
    [SerializeField] private Player _player;
    [SerializeField] private List<ItemData> _allItems = new List<ItemData>();
    [SerializeField] private GameObject _environment;

    private float _timeLeft;

    public List<ItemData> AllItems => _allItems;
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

    public void EnterNewArea()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Transition()
    {
        UIManager.i.Do(UIAction.SHOW_TRANSITION);
        _environment.SetActive(false);
    }

    public void LoseGame()
    {
        _player.Die();
        UIManager.i.Do(UIAction.SHOW_DEATH_SCREEN);
    }
}
