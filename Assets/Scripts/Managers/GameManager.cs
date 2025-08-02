using MyBox;
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

    [SerializeField] private float _totalTime;
    [SerializeField] private Player _player;
    [SerializeField] private Car _car;
    [SerializeField] private CameraController _camera;
    [SerializeField] private List<ItemData> _allItems = new List<ItemData>();
    [SerializeField] private List<ItemType> _discoveredItems = new List<ItemType>();
    [SerializeField] private GameObject _environment;
    [SerializeField] private AreaController _areaController;
    [SerializeField] private List<GameObject> _areaPrefabs = new List<GameObject>();

    private float _timeLeft;
    private bool _timerPaused;


    public bool ItemDiscovered(ItemType type) => _discoveredItems.Contains(type);
    public List<ItemData> AllItems => _allItems;
    public void LoadMenu() => Utils.TransitionToScene(0);
    public void EndGame() => Utils.TransitionToScene(2);
    public Player Player => _player;
    public Transform Camera => _camera.transform;
    public void ResumeTimer() => _timerPaused = false;
    public void PauseTimer() => _timerPaused = true;
    public string GetDisplayName(ItemType type) => AllItems.Where(x => x.Type == type).First().DisplayName;

    private void Awake()
    {
        if (i != null) Destroy(i.gameObject);
        i = this;
    }

    private void Start()
    {

        Utils.SetCursor(false);
        _timeLeft = _totalTime;

        if (_areaController) _areaController.InitializeArea(_car);
        else EnterNewArea();
    }

    private void Update()
    {
        if (_player.Dead) return;

        if (!_timerPaused) _timeLeft -= Time.deltaTime;
        UIManager.i.Do(UIAction.UPDATE_TIMER, _timeLeft / _totalTime);

        if (_timeLeft <= 0) LoseGame();
    }
    public void DiscoverItem(ItemType type)
    {
        if (!_discoveredItems.Contains(type)) {
            var itemName = GetDisplayName(type);
            UIManager.i.Do(UIAction.SHOW_POPUP, "discovered " + itemName);
            _discoveredItems.Add(type);
        }
    }

    public void EnterNewArea()
    {
        if (_areaController) Destroy(_areaController.gameObject);

        var selectedArea = _areaPrefabs[Random.Range(0, _areaPrefabs.Count)];
        _areaController = Instantiate(selectedArea, _environment.transform).GetComponent<AreaController>();

        _player.gameObject.SetActive(false);
        _camera.FollowCar();
        _car.StartDriving();

        _areaController.EnterArea(_car, _camera);
        _environment.SetActive(true);

        _timeLeft = _totalTime;
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
