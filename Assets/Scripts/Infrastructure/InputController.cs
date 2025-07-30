using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField, DisplayInspector] private ControlsSaveData _data;

    public static InputController i;

    public static bool GetDown(Control key) => i.GetKeyDownInternal(key);
    public static bool Get(Control key) => i.GetKeyInternal(key);
    public static bool GetUp(Control key) => i.GetKeyUpInternal(key);

    private void OnValidate()
    {
        _data.OnValidate();
    }

    private void Awake()
    {
        i = this;        
    }

    public List<MappedKeyData> GetCurrent() => _data.Controls;

    private MappedKeyData GetDataByControl(Control control)
    {
        var selected = _data.Controls.Where(x => x.Control == control).FirstOrDefault();
        return selected;
    }

    private bool GetKeyDownInternal(Control key)
    {
        var selected = GetDataByControl(key);
        if (selected == default) return false;
        return Input.GetKeyDown(selected.Key);
    }

    private bool GetKeyInternal(Control key)
    {
        var selected = GetDataByControl(key);
        if (selected == default) return false;
        return Input.GetKey(selected.Key);
    }

    private bool GetKeyUpInternal(Control key)
    {
        var selected = GetDataByControl(key);
        if (selected == default) return false;
        return Input.GetKeyUp(selected.Key);
    }

    public KeyCode GetKeyCode(Control control)
    {
        var data = GetDataByControl(control);
        return data.Key;
    }

    public void RebindKey(Control control, KeyCode key)
    {
        foreach (var c in _data.Controls) {
            if (c.Control != control) continue;

            c.Key = key;
            SaveControl(c.Control);
        }
    }

    private void SaveControl(Control control)
    {
        var selected = GetDataByControl(control);
        if (selected == default) return;

#if UNITY_EDITOR
        EditorUtility.SetDirty(_data);
#endif

    }
}
