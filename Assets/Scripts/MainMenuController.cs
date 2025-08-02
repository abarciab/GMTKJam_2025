using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MusicPlayer _music;
    [SerializeField] private TextAsset _introTextFile;
    [SerializeField] private DialogueController _dialogue;

    private void Start()
    {
        Utils.SetCursor(true);
        var introTextLines = _introTextFile.text.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        _dialogue.StartConversation(introTextLines);
    }

    public void StartGame()
    {
        Utils.TransitionToScene(1);
        Utils.SetCursor(false);
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
