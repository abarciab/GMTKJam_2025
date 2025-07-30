using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MusicPlayer _music;

    public void StartGame()
    {
        Utils.TransitionToScene(1);
    }

    public void Quit()
    {
        _music.FadeOutCurrent(Utils.fadeTime);
        UIManager.i.Do(UIAction.FADE_TO_BLACK);

#if !UNITY_EDITOR
        Invoke(nameof(Application.Quit), Utils.fadeTime);
#endif
    }
}
