using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField, TextArea(3, 10)] private string _text;

    public string Name => _name;

    public void StartConversation()
    {
        UIManager.i.Do(UIAction.START_CONVERSATION, _text);
    }
}
