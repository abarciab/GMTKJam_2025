using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : UIController
{
    [SerializeField] private TextMeshProUGUI _collectibleNameText;
    [SerializeField] private TextMeshProUGUI _numRocksText;
    [SerializeField] private TextMeshProUGUI _carRocksText;
    [SerializeField] private Slider _breakSlider;

    private void Start()
    {
        DisplayNumRocks(0);
        DisplayCollectible("");
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.DISPLAY_COLLECTABLE && arg is string collectableName) DisplayCollectible(collectableName);
        if (action == UIAction.DISPLAY_ROCK_TOTAL && arg is int numRocks) DisplayNumRocks(numRocks);
        if (action == UIAction.DISPLAY_CAR_ROCKS && arg is Vector2Int carRockNums) DisplayCarRocks(carRockNums);
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
    }

    private void DisplayNumRocks(int numRocks)
    {
        _numRocksText.text = "Rocks: " + numRocks;
    }

    private void DisplayCarRocks(Vector2Int carRocks)
    {
        _carRocksText.text = "Car Rocks: " + carRocks.x + "/" + carRocks.y;
    }
}
