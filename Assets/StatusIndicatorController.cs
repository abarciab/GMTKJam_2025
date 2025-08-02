using System.Collections.Generic;
using UnityEngine;

public enum Status { ENCUMBERED, LOW_FUEL, OUT_OF_FUEL, LOW_HEALTH}

[System.Serializable]
public class StatusText
{
    [HideInInspector] public string Name;
    public Status Status;
    public GameObject Text;
}

public class StatusIndicatorController : UIController
{
    [SerializeField] private List<StatusText> _statusTexts = new List<StatusText>();

    private void OnValidate()
    {
        foreach (var text in _statusTexts) text.Name = text.Status.ToString();
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_STATUS && arg is Status showStatus) SetStatus(showStatus, true); 
        if (action == UIAction.HIDE_STATUS && arg is Status hideStatus) SetStatus(hideStatus, false); 
    }

    private void SetStatus(Status status, bool active)
    {
        foreach (var t in _statusTexts) if (t.Status == status) t.Text.SetActive(active);                
    }
}
