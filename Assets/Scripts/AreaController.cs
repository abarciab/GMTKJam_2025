using MyBox;
using SplineMeshTools.Core;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Trade
{
    public Inventory Request = new Inventory();
    public Inventory Offer = new Inventory();
    
    public Trade() {}
    public Trade(Trade other)
    {
        Request = new Inventory();
        foreach (var item in other.Request.Items) {
            Request.AddItems(new Item(item));
        }
        Offer = new Inventory();
        foreach (var item in other.Offer.Items) {
            Offer.AddItems(new Item(item));
        }
    }
}

public class AreaController : MonoBehaviour
{
    public string Name;
    public int roadIndex;
    [SerializeField] private Transform _entrance;
    [SerializeField] private Transform _endGate;
    [SerializeField] private SplineMesh _road;

    [Header("NPCs")]
    [SerializeField] private TextAsset _textFile;
    [SerializeField] private List<Trade> _trades;
    [SerializeField] private Transform _npcParent;
    [SerializeField] private int _numNPCS = 3;
    private List<List<string>> _conversations = new List<List<string>>();
    [ReadOnly, SerializeField] private int _numConversations;

    [Header("Player Truck")]
    [SerializeField] private int _startingFuel = 5;

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
            selected.SetData(GetLines(), GetTrade());
            allNpcs.Remove(selected);
            if (allNpcs.Count == 0) break;
        }

        foreach (var npc in allNpcs) Destroy(npc.gameObject);
    }

    [ButtonMethod]
    private void ParseTextFile()
    {
        _conversations.Clear();

        var text = _textFile.text;
        var conversations = text.Split("[]").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        foreach (var conversation in conversations) {
            var lines = conversation.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            _conversations.Add(lines);
        }

        _numConversations = _conversations.Count;
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

    public Trade GetTrade()
    {
        if (_trades.Count > 0) {
            var selected = _trades[Random.Range(0, _trades.Count)];
            _trades.Remove(selected);
            return selected;
        }
        else {
            return new Trade();
        }
    }

    public void InitializeArea(Car car)
    {
        car.SetEndGate(_endGate);
        //_road.gameObject.SetActive(false);
        //_road.gameObject.SetActive(true);
    }

    public void EnterArea(Car car, CameraController camera)
    {
        car.transform.SetPositionAndRotation(_entrance.position, _entrance.rotation);
        car.transform.position = _entrance.position;

        camera.FollowCar();
        car.SetFuel(_startingFuel);
        car.SetThrottle(3);
        car.setWheelAngle(0);
        InitializeArea(car);
        FindFirstObjectByType<Player>(FindObjectsInactive.Include).GetInCar();
        car.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        camera.SnapToCar();
    }
}
