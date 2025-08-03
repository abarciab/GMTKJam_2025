using UnityEngine;

public class PopupController : UIController
{
    [SerializeField] private GameObject _popupPrefab;
    [SerializeField] private Transform _listParent;

    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_POPUP && arg is string text) ShowPopup(text);
    }

    private void ShowPopup(string text)
    {
        var newPopup = Instantiate(_popupPrefab, _listParent).GetComponent<TextPopup>();
        newPopup.Initialize(text);
    }

}
