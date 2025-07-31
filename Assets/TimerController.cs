using UnityEngine;
using UnityEngine.UI;

public class TimerController : UIController
{
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private Image _fillImg;
    [SerializeField] private Gradient _colorGradient;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.UPDATE_TIMER && arg is float timePercent) {
            _timerSlider.value = timePercent;
            _fillImg.color = _colorGradient.Evaluate(timePercent);
        }
    }
}
