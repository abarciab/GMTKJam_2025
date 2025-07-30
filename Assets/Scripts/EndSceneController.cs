using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] RectTransform _contentParent;
    [SerializeField] float _startPos = -250;
    [SerializeField] float _scrollSpeed;

    private void Start()
    {
        UIManager.i.Do(UIAction.FADE_FROM_BLACK);

        var pos = _contentParent.anchoredPosition;
        pos.y = _startPos;
        _contentParent.anchoredPosition = pos;
    }

    private void Update()
    {
        if (_contentParent.anchoredPosition.y < _contentParent.rect.height) {
            _contentParent.anchoredPosition += _scrollSpeed * Time.deltaTime * Vector2.up;
            if (_contentParent.anchoredPosition.y > _contentParent.rect.height) _contentParent.anchoredPosition = new Vector2(0, _contentParent.rect.height);
        }
    }

    public void GoToMenu()
    {
        Utils.TransitionToScene(0);
    }
}
