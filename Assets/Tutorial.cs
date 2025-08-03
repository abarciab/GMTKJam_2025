using MyBox;
using System.Collections.Generic;
using TMPro;
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

    [Header("References")]
    [SerializeField] private Car _car;
    [SerializeField] private Player _player;

    private float _timePassed;

    private void Start()
    {
        if (!_enabled) {
            _player.GetInCar();
            enabled = false;
        }
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_STATUS && arg is Status status && status == Status.FUEL_EMPTY) {
            Show(_outOfFuel);
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

        if (_player.gameObject.activeInHierarchy && _outOfFuel.Completed && !_howToWalk.Completed) {
            Show(_howToWalk);
        }
    }

    private void Show(TutorialLine line)
    {
        if (line.Completed) return;

        line.Completed = true;
        UIManager.i.Do(UIAction.START_CONVERSATION, line.Lines);
    }

}
