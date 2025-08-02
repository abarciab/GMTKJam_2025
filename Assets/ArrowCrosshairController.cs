using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RTransformLerpPositionData
{
    [HideInInspector] public string Name;
    public RectTransform RTransform;
    public Vector2 StartPos;
    public Vector2 EndPos;

    public void UpdatePosition(float percent)
    {
        RTransform.gameObject.SetActive(percent > 0);   
        RTransform.anchoredPosition = Vector2.Lerp(StartPos, EndPos, percent);
    }

}

public class ArrowCrosshairController : UIController
{
    [SerializeField] private Image _mainCrosshair;
    [SerializeField] private List<RTransformLerpPositionData> _parts = new List<RTransformLerpPositionData>();

    private void OnValidate()
    {
        foreach (var part in _parts) if (part.RTransform) part.Name = part.RTransform.gameObject.name;
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_ARROW_PROGRESS && arg is float arrowProgress) ShowProgress(arrowProgress);
    }

    private void ShowProgress(float percent)
    {
        _mainCrosshair.enabled = percent == 0;

        foreach (var point in _parts) {
            point.UpdatePosition(percent);
        }
    }
}
