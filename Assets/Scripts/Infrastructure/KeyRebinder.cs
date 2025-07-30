using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyRebinder : MonoBehaviour
{
    [SerializeField] private Transform _listParent;
    [SerializeField] private GameObject _listEntryPrefab;
    private List<Transform> _spawnedButtons = new List<Transform>();

    [SerializeField] private bool _listening;
    private Control _currentTarget = Control.NONE;
    private TextMeshProUGUI _currentTargetText;

    private void Start()
    {
        ClearButtons();
        SpawnButtons();
        _listening = false;
    }

    private void OnDisable()
    {
        _listening = false;
    }

    private void ClearButtons()
    {
        for (int i = _listParent.childCount - 1; i > 0; i--) {
            var child = _listParent.GetChild(i).gameObject;
            if (child.GetComponentInChildren<Button>() != null) Destroy(child);
        }
    }

    private void SpawnButtons()
    {
        var current = InputController.i.GetCurrent();
        for (int i = 0; i < current.Count; i++) {
            SpawnButton(current[i], i);
        }
        
    }

    private void SpawnButton(MappedKeyData data, int index)
    {
        var even = index % 2 == 0;
        var newButton = Instantiate(_listEntryPrefab, _listParent);
        var textComponents = newButton.GetComponentsInChildren<TextMeshProUGUI>();
        textComponents[0].text = data.DisplayName;
        textComponents[1].text = data.Key.ToString();
        newButton.GetComponentInChildren<Image>().enabled = !even;
        newButton.transform.SetSiblingIndex(index + 1);
        newButton.GetComponentInChildren<Button>().onClick.AddListener(() => StartListening(data.Control, textComponents[1]));
    }

    public void Update()
    {
        if (!_listening) return;

        var key = CheckForKey();
        if (key != KeyCode.None) SetNewKey(key);
    }

    private void SetNewKey(KeyCode key)
    {
        InputController.i.RebindKey(_currentTarget, key);
        _currentTargetText.text = key.ToString();
        _listening = false;
    }

    private KeyCode CheckForKey()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKey(kcode)) return kcode;
        }
        return KeyCode.None;
    }

    public void StartListening(Control control, TextMeshProUGUI keyText)
    {
        if (_listening) {
            _currentTargetText.text = InputController.i.GetKeyCode(_currentTarget).ToString();

            if (control == _currentTarget) {
                _listening = false;
                return;
            }
        }

        _currentTargetText = keyText;
        keyText.text = "";
        _currentTarget = control;
        _listening = true;
    }
}
