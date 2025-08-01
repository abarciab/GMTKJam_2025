using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private TextAsset _textFile;
    
    private List<string> _lines = null;

    public string Name => _name;

    public void SetData(List<string> lines)
    {
        _name = lines[0];
        lines.RemoveAt(0);
        _lines = lines;
    }

    public void StartConversation()
    {
        UIManager.i.Do(UIAction.START_CONVERSATION, _lines);
    }
}
