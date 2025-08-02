using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarRepairSubMenuPanel : MonoBehaviour
{
    [SerializeField] private List<InventoryItemDisplay> _itemDisplays = new List<InventoryItemDisplay>();
    [SerializeField] private Inventory _requiredItems;
    [SerializeField] private SelectableItem _button;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _maxText;

    private CarRepairMenuController _controller;
    private float _targetSliderValue;

    public void Initialize(float percent, CarRepairMenuController controller)
    {
        _maxText.SetActive(percent == 1);
        _button.gameObject.SetActive(!_maxText.activeInHierarchy);

        _controller = controller;
        _targetSliderValue = percent;

        var requiredItems = _requiredItems.Items;
        for (int i = 0; i < _itemDisplays.Count; i++) {
            _itemDisplays[i].gameObject.SetActive(requiredItems.Count > i);
            if (_itemDisplays[i].gameObject.activeInHierarchy) {
                _itemDisplays[i].Initialize(requiredItems[i]);
            }
        }

        var inventory = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory;
        
        _button.SetDisabled(!inventory.Contains(requiredItems));
    }

    private void Update()
    {
        if (_slider.value > _targetSliderValue) _slider.value = _targetSliderValue;
        _slider.value = Mathf.Lerp(_slider.value, _targetSliderValue, 4 * Time.deltaTime);
    }

    public void ClickButton()
    {
        var inventory = GameManager.i.Player.GetComponent<PlayerInventory>().Inventory;
        inventory.RemoveItems(_requiredItems.Items);

        _controller.Purchase(this);
    }

}
