using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ControlData", menuName = "ControlsData")]
public class ControlsSaveData : ScriptableObject
{
    public List<MappedKeyData> Controls;

    public void OnValidate()
    {
        foreach (Control control in System.Enum.GetValues(typeof(Control))) {
            if (control == Control.NONE) continue;
            bool missing = true;
            foreach (var c in Controls) if (c.Control == control) missing = false;
            if (missing) Controls.Add(new MappedKeyData(control));
        }
    }
}
