using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private TextMeshProUGUI _text2;

    public void Initialize(string text)
    {
        _text1.text = _text2.text = text;
    }
}
