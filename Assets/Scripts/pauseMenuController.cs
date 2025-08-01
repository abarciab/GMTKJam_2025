using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenuController : UIController
{
    [SerializeField] private Sound _clickSound;
    [SerializeField] private GameObject _settings;

    private void Toggle() => SetPaused(Time.timeScale > 0f);
    public void ToggleSettings() => _settings.SetActive(!_settings.activeInHierarchy);

    public void Resume() => SetPaused(false);
    private void OnEnable()
    {
        _settings.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _clickSound = Instantiate(_clickSound);
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.TOGGLE_PAUSE) Toggle();
        if (action == UIAction.PAUSE) SetPaused(true);
        if (action == UIAction.RESUME) SetPaused(false);
    }

    public void SetPaused(bool paused)
    {
        gameObject.SetActive(paused);
        AudioManager.i.SetPaused(paused);
        Time.timeScale = paused ? 0 : 1;
    }

    public void ExitToMainMenu()
    {
        SetPaused(false);
        GameManager.i.LoadMenu();
        _clickSound.Play();
    }

}
