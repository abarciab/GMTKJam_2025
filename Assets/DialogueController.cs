using TMPro;
using UnityEngine;

public class DialogueController : UIController
{
    [SerializeField] private TextMeshProUGUI _mainText;

    private void OnEnable()
    {
        Utils.SetCursor(true);
    }

    private void OnDisable()
    {
        GameManager.i.ResumeTimer();
        Utils.SetCursor(false);
    }

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.START_CONVERSATION && arg is string text) StartConversation(text);
    }

    private void StartConversation(string text)
    {
        GameManager.i.PauseTimer();
        gameObject.SetActive(true);
        _mainText.text = text;
    }

    public void ShowNext()
    {
        EndConversation();
    }

    private void EndConversation()
    {
        gameObject.SetActive(false);
        FindFirstObjectByType<CameraController>().EndConversation();
        FindFirstObjectByType<Player>().EndConversation();
    }

    
}
