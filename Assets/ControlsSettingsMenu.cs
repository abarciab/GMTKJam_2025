using UnityEngine;
using UnityEngine.UI;

public class ControlsSettingsMenu : MonoBehaviour
{
    [SerializeField] private DropdownOption _sprintMode;

    [SerializeField] private Slider _mouseSensitivityX;
    [SerializeField] private Slider _mouseSensitivityY;

    private void Start()
    {
        _mouseSensitivityX.onValueChanged.AddListener(ChangeSensitivitySliders);
        _mouseSensitivityY.onValueChanged.AddListener(ChangeSensitivitySliders);

        _mouseSensitivityX.value = Settings.MouseSensitivity.x / 2;
        _mouseSensitivityY.value = Settings.MouseSensitivity.y / 2;
    }


    private void OnEnable()
    {
        _sprintMode.Initialize(SprintOptions.HOLD);
        _mouseSensitivityX.value = Settings.MouseSensitivity.x / 2;
        _mouseSensitivityY.value = Settings.MouseSensitivity.y / 2;
    }

    public void ChangeSensitivitySliders(float value)
    {
        Settings.MouseSensitivity = new Vector2(_mouseSensitivityX.value, _mouseSensitivityY.value) * 2;
    }
}
