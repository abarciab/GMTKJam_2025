using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider _masterVolume;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _ambientVolume;
    [SerializeField] private Slider _sfxVolume;
    [SerializeField] private Slider _mouseSensitivityX;
    [SerializeField] private Slider _mouseSensitivityY;

    private void Start()
    {
        _masterVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMasterVolume(value));
        _musicVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMusicVolume(value));
        _ambientVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetAmbientVolume(value));
        _sfxVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetSFXVolume(value));

        _mouseSensitivityX.value = Utils.MouseSensitivity.x / 2;
        _mouseSensitivityY.value = Utils.MouseSensitivity.y / 2;
    }

    private void OnEnable()
    {
        SetSliderValuesToAudioSettings();
        _mouseSensitivityX.value = Utils.MouseSensitivity.x / 2;
        _mouseSensitivityY.value = Utils.MouseSensitivity.y / 2;
    }

    public void ChangeSensitivitySliders()
    {
        Utils.MouseSensitivity = new Vector2(_mouseSensitivityX.value, _mouseSensitivityY.value) * 2;
    }

    void SetSliderValuesToAudioSettings()
    {
        var volumes = AudioManager.i.GetVolumes();
        _masterVolume.value = volumes[0];
        _musicVolume.value = volumes[1];
        _ambientVolume.value = volumes[2];
        _sfxVolume.value = volumes[3];
    }
}
