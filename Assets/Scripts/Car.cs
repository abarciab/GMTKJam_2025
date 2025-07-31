using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Inventory _currentInventory;
    [SerializeField] private Inventory _requiredInventory;
    [SerializeField] private Sound _depositSound;

    public bool ReadyToGo => _currentInventory.Contains(_requiredInventory);

    private void Awake()
    {
        _depositSound = Instantiate(_depositSound);
    }

    private void Start()
    {
        UpdateUI();
    }

    public List<Item> GetRemainingRequiredItems()
    {
        return _currentInventory.GetDifference(_requiredInventory);
    }

    public string GetDisplayString(Inventory playerInventory)
    {
        if (ReadyToGo) return "Leave Area";
        else {
            var readyToDeposit = playerInventory.GetOverlap(GetRemainingRequiredItems()).Count > 0;
            if (readyToDeposit) return "Deposit Items";
            else return "More items required";
        }
    }

    public void DepositItems(List<Item> toDeposit)
    {
        foreach (var i in toDeposit) {
            _currentInventory.Additems(i);
        }

        _depositSound.Play();

        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.i.Do(UIAction.DISPLAY_CAR_INVENTORY, GetRemainingRequiredItems());
    }
}
