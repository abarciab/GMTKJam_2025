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

    [Header("Timer")]
    [SerializeField] private float _totalTime;
    [SerializeField] private Transform _creatureParent;
    [SerializeField, Range(0, 1)] private List<float> _spawnTimes;

    [Header("Misc")]
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
    private List<float> _originalSpawnTimes;

    public bool ItemDiscovered(ItemType type) => _discoveredItems.Contains(type);
    public List<ItemData> AllItems => _allItems;
    public void LoadMenu() => Utils.TransitionToScene(0);
    public void EndGame() => Utils.TransitionToScene(2);
    public Player Player => _player;
    public Car Car => _car;
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
        _originalSpawnTimes = new List<float>(_spawnTimes);

        Utils.SetCursor(false);
        _timeLeft = _totalTime;

        if (_areaController) _areaController.EnterArea(_car, _camera);
        else EnterNewArea();
    }

    private void Update()
    {
        if (_player.Dead) return;

        if (!_timerPaused) _timeLeft -= Time.deltaTime;
        var timerPercent = _timeLeft / _totalTime;
        UIManager.i.Do(UIAction.UPDATE_TIMER, timerPercent);

        if (_spawnTimes.Count > 0 && timerPercent <= _spawnTimes[0]) {
            foreach (Transform child in _creatureParent) {
                if (!child.gameObject.activeInHierarchy) { 
                    child.gameObject.SetActive(true); 
                    break; 
                }
            }
            _spawnTimes.RemoveAt(0);
        }

        //if (_timeLeft <= 0) LoseGame();
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
        print("gamemanager enter area");

        _car.GetComponent<Rigidbody>().isKinematic = true;

        _spawnTimes = new List<float>(_originalSpawnTimes);
        foreach (Transform child in _creatureParent) child.gameObject.SetActive(false);
        
        if (_areaController) Destroy(_areaController.gameObject);

        var selectedArea = _areaPrefabs[Random.Range(0, _areaPrefabs.Count)];
        _areaController = Instantiate(selectedArea, _environment.transform).GetComponent<AreaController>();

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
