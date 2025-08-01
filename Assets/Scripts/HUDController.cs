using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : UIController
{
    [SerializeField] private TextMeshProUGUI _collectibleNameText;
    [SerializeField] private Slider _breakSlider;

    private void Start()
    {
        DisplayCollectible("");
    }    

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.DISPLAY_HOVERED && arg is string collectableName) DisplayCollectible(collectableName);
        if (action == UIAction.SHOW_BREAK_PROGRESS && arg is float breakProgress) DisplayBreakProgress(breakProgress);
    }

    private void DisplayBreakProgress(float progress)
    {
        _breakSlider.gameObject.SetActive(progress > 0);
        _breakSlider.value = progress;
    }

    private void DisplayCollectible(string collectableName)
    {
        _collectibleNameText.text = collectableName;
        if (string.IsNullOrEmpty(collectableName)) _breakSlider.gameObject.SetActive(false);
    }
}
