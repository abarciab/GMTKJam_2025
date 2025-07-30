using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    [SerializeField] private bool _oneFrameDelay;

    public void Toggle()
    {
        if (_oneFrameDelay) StartCoroutine(WaitThenToggle());
        else gameObject.SetActive(!gameObject.gameObject.activeInHierarchy);
    }

    private IEnumerator WaitThenToggle()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(!gameObject.gameObject.activeInHierarchy);
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
    public void Set(bool state) => gameObject.SetActive(state);
}
