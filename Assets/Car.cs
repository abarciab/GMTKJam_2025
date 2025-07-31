using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private int _numRocks;
    [SerializeField] private int _numRocksRequired = 10;
    [SerializeField] private Sound _depositSound;

    public bool ReadyToGo => _numRocks >= _numRocksRequired;

    private void Awake()
    {
        _depositSound = Instantiate(_depositSound);
    }

    private void Start()
    {
        UpdateUI();
    }

    public string GetDisplayString(int numRocks)
    {
        if (_numRocks == _numRocksRequired) return "Leave Area";
        if (_numRocks < _numRocksRequired) {
            if (numRocks == 0) return "Rocks required";
            else return "Deposit Rocks";
        }

        return "Car";
    }

    public int DepositRocks(int numNewRocks)
    {
        var amountAccepted = Mathf.Min(numNewRocks, _numRocksRequired - _numRocks);
        if (amountAccepted == 0) return 0;

        _depositSound.Play();

        _numRocks += numNewRocks;
        _numRocks = Mathf.Min(_numRocks, _numRocksRequired);

        UpdateUI();

        return amountAccepted;
    }

    private void UpdateUI()
    {
        UIManager.i.Do(UIAction.DISPLAY_CAR_ROCKS, new Vector2Int(_numRocks, _numRocksRequired));
    }
}
