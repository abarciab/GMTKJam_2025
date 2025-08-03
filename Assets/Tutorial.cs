using MyBox;
using System.Collections.Generic;
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

    [Header("References")]
    [SerializeField] private Car _car;
    [SerializeField] private Player _player;

    private Inventory _playerInventory;

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

    }

    private void Update()
    {
        _timePassed += Time.deltaTime;

        if (!_intro.Completed && _timePassed > 1.5f) {
            Show(_intro);
            _player.GetInCar();
            //_car.StartDriving();
        }

        if (_player.gameObject.activeInHierarchy && !Cursor.visible) {
            if (_outOfFuel.Completed && !_howToWalk.Completed) Show(_howToWalk);
            if (_refuel.Completed && !_flintPine.Completed) Show(_flintPine);

            if (_playerInventory.GetCount(ItemType.FLINT) >= 1 && _playerInventory.GetCount(ItemType.HOLLOW_PINE) >= 2) Show(_readyToRefuel);
        }
        if (_car.FuelPercent > 0.2f && _readyToRefuel.Completed) Show(_readyToLeave);
    }

    private void Show(TutorialLine line)
    {
        if (line.Completed) return;

        line.Completed = true;
        UIManager.i.Do(UIAction.START_CONVERSATION, line.Lines);
    }

}
