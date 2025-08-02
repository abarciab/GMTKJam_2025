using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField] private Transform _entrance;
    [SerializeField] private Transform _endGate;

    [Header("NPCs")]
    [SerializeField] private TextAsset _textFile;
    [SerializeField] private Transform _npcParent;
    [SerializeField] private int _numNPCS = 3;
    private List<List<string>> _conversations = new List<List<string>>();

    private void Start()
    {
        if (_textFile) SpawnNPCs(); 
        else Destroy(_npcParent.gameObject);
    }

    private void SpawnNPCs()
    {
        ParseTextFile();

        var allNpcs = _npcParent.GetComponentsInChildren<NPC>().ToList();

        for (int i = 0; i < _numNPCS; i++) {
            var selected = allNpcs[Random.Range(0, allNpcs.Count())];
            selected.SetData(GetLines());
            allNpcs.Remove(selected);
            if (allNpcs.Count == 0) break;
        }

        foreach (var npc in allNpcs) Destroy(npc.gameObject);
    }

    private void ParseTextFile()
    {
        _conversations.Clear();

        var text = _textFile.text;
        var conversations = text.Split("[]").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        foreach (var conversation in conversations) {
            var lines = conversation.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            _conversations.Add(lines);
        }
    }

    public List<string> GetLines()
    {
        if (_conversations.Count > 0) {
            var selected = _conversations[Random.Range(0, _conversations.Count)];
            _conversations.Remove(selected);
            return selected;
        }
        else {
            return new List<string>() { "Conversation not found" };
        }
    }

    public void InitializeArea(Car car)
    {
        car.SetEndGate(_endGate);
    }

    public void EnterArea(Car car, CameraController camera)
    {
        car.transform.SetPositionAndRotation(_entrance.position, _entrance.rotation);
        
        car.SetFuel(3);
        car.SetThrottle(3);
        car.setWheelAngle(0);
        InitializeArea(car);

        camera.SnapToCar();

    }
}
