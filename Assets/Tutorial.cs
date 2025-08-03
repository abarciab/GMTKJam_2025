using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class TutorialLine
{
    public bool Completed;
    [TextArea(3, 10)] public List<string> Lines = new List<string>();
}

public class Tutorial : UIController
{
    [SerializeField] private bool _enabled = true;

    [Header("Lines")]
    [SerializeField] private TutorialLine _intro;
    [SerializeField] private TutorialLine _outOfFuel;
    [SerializeField] private TutorialLine _howToWalk;
    [SerializeField] private TutorialLine _refuel;
    [SerializeField] private TutorialLine _flintPine;
    [SerializeField] private TutorialLine _readyToRefuel;
    [SerializeField] private TutorialLine _readyToLeave;
    [SerializeField] private TutorialLine _npcIntro;
    [SerializeField] private TutorialLine _npcTrade;
    [SerializeField] private TutorialLine _rareItems;
    [SerializeField] private TutorialLine _checkHood;
    [SerializeField] private TutorialLine _carUpgrade;
    [SerializeField] private TutorialLine _creature;
    [SerializeField] private TutorialLine _wreck;
    [SerializeField] private TutorialLine _postScrap;
    [SerializeField] private TutorialLine _huntingGroundInitial;
    [SerializeField] private TutorialLine _spirit;
    [SerializeField] private TutorialLine _postSpirit;

    [Header("Parameters")]
    [SerializeField] private float _npcDistance = 50;
    [SerializeField] private float _huntingGroundDistance = 65;
    [SerializeField] private float _spiritDistance = 30;
    [SerializeField] private float _wreckDistance = 50;
    [SerializeField] private float _creatureDistance = 50;

    [Header("References")]
    [SerializeField] private Car _car;
    [SerializeField] private Player _player;
    [SerializeField] private CameraController _camera;

    private Inventory _playerInventory;

    private bool _readyForTrade;
    private float _timePassed;

    private void Start()
    {
        if (!_enabled) {
            _player.GetInCar();
            enabled = false;
        }
        _playerInventory = _player.GetComponent<PlayerInventory>().Inventory(InventoryType.PLAYER);
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_STATUS && arg is Status status && status == Status.FUEL_EMPTY) {
            Show(_outOfFuel);
        }
        if (action == UIAction.OPEN_REPAIR_MENU) {
            Show(_refuel);
        }
        if (action == UIAction.FINISH_NPC_CONVERSATION) {
            _readyForTrade = true;
        }
        if (action == UIAction.START_TRADE) {
            Show(_rareItems);
        }
        if (action == UIAction.END_TRADE) {
            Show(_checkHood);
        }
        if (action == UIAction.SHOW_CAR_UPGRADE) {
            Show(_carUpgrade);
        }
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;

        if (!_intro.Completed && _timePassed > 1.5f) {
            Show(_intro);
        }

        if (_player.gameObject.activeInHierarchy && !Cursor.visible) {
            if (_outOfFuel.Completed && !_howToWalk.Completed) {
                _camera.SnapToPlayer();
                Show(_howToWalk);
            }
            if (_refuel.Completed && !_flintPine.Completed) Show(_flintPine);

            if (_playerInventory.GetCount(ItemType.FLINT) >= 1 && _playerInventory.GetCount(ItemType.HOLLOW_PINE) >= 2) {
                Show(_readyToRefuel);
                _flintPine.Completed = true;
            }

            if (_huntingGroundInitial.Completed && !_spirit.Completed && FindObjectsByType<Animal>(FindObjectsSortMode.None).Length > 0) {
                var closest = FindObjectsByType<Animal>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, _player.transform.position)).First();
                if (Vector3.Distance(closest.transform.position, _player.transform.position) < _spiritDistance) {
                    Show(_spirit);
                }
            }


        }
        if (_car.FuelPercent > 0.2f && _readyToRefuel.Completed) {
            Show(_readyToLeave);
            _flintPine.Completed = true;
        }

        if (!_npcIntro.Completed) {
            var closest = FindObjectsByType<NPC>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, _player.transform.position)).First();
            if (Vector3.Distance(closest.transform.position, _player.transform.position) < _npcDistance) {
                Show(_npcIntro);
            }
        }
        
        if (!Cursor.visible && !_creature.Completed && FindObjectsByType<Foot>(FindObjectsSortMode.None).Length > 0) {
            var closest = FindObjectsByType<Foot>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, _player.transform.position)).First();
            if (Vector3.Distance(closest.transform.position, _player.transform.position) < _creatureDistance) {
                Show(_creature);
                Camera.main.transform.LookAt(closest.transform.position + Vector3.up * 50);
            }
        }

        if (!_wreck.Completed) {
            var closest = FindObjectsByType<Collectable>(FindObjectsSortMode.None).Where(x => x.gameObject.CompareTag("Wreck")).OrderBy(x => Vector3.Distance(x.transform.position, _player.transform.position)).First();
            if (Vector3.Distance(closest.transform.position, _player.transform.position) < _wreckDistance) {
                Show(_wreck);
            }
        }       

        if (!_postScrap.Completed) {
            if (_playerInventory.GetCount(ItemType.GLASS) > 0 || _playerInventory.GetCount(ItemType.SCRAP) > 0 || _playerInventory.GetCount(ItemType.CLOTH) > 0 || _playerInventory.GetCount(ItemType.WIRE) > 0) {
                Show(_postScrap);
            }
        }

        if (!_huntingGroundInitial.Completed) {
            var closest = FindObjectsByType<HuntingGround>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, _player.transform.position)).First();
            if (Vector3.Distance(closest.transform.position, _player.transform.position) < _huntingGroundDistance) {
                Show(_huntingGroundInitial);
            }
        }
        

        if (_playerInventory.GetCount(ItemType.SOUL_SHARD) > 0 || _playerInventory.GetCount(ItemType.SOUL) > 0) {
            Show(_postSpirit);
        }
    }

    private void Show(TutorialLine line)
    {
        if (line.Completed) return;
        line.Completed = true;
        if (!_enabled) return;

        UIManager.i.Do(UIAction.DISPLAY_HOVERED, "");
        UIManager.i.Do(UIAction.SHOW_BREAK_PROGRESS, 0);

        UIManager.i.Do(UIAction.START_CONVERSATION, line.Lines);
    }

}
