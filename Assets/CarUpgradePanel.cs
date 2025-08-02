using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarUpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject _requirements;
    [SerializeField] private CarStatType _stat;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Slider _slider;

    public void SetHover(bool hovered) => _requirements.SetActive(hovered);

    private void OnEnable()
    {
        var car = FindFirstObjectByType<Car>();

        var currentStat = car.GetStat(_stat);
        _nameText.text = currentStat.Data.DisplayName;
        _descriptionText.text = currentStat.Data.Description;
        _slider.value = currentStat.Value / currentStat.Data.MaxValue;

    }
}
