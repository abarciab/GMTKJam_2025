using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : UIController
{
    [SerializeField] private TextMeshProUGUI _collectibleNameText;
    [SerializeField] private Slider _breakSlider;
    [SerializeField] private Slider _staminaBar;

    private void Start()
    {
        DisplayCollectible("");
        DisplayStaminaProgress(0);
    }    

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.DISPLAY_HOVERED && arg is string collectableName) DisplayCollectible(collectableName);
        if (action == UIAction.SHOW_BREAK_PROGRESS && arg is float breakProgress) DisplayBreakProgress(breakProgress);
        if (action == UIAction.SHOW_STAMINA && arg is float staminaPercent) DisplayStaminaProgress(staminaPercent);
    }

    private void DisplayBreakProgress(float progress)
    {
        _breakSlider.gameObject.SetActive(progress > 0);
        _breakSlider.value = progress;
    }

    private void DisplayStaminaProgress(float progress)
    {
        _staminaBar.gameObject.SetActive(progress > 0);
        _staminaBar.value = progress;
    }

    private void DisplayCollectible(string collectableName)
    {
        _collectibleNameText.text = collectableName;
        if (string.IsNullOrEmpty(collectableName)) _breakSlider.gameObject.SetActive(false);
    }
}
