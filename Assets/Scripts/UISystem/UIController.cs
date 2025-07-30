using UnityEngine;

public class UIController : MonoBehaviour
{
    public void Initialize()
    {
        UIManager.i.OnUpdateUI.AddListener(UpdateUI);
    }

    protected virtual void UpdateUI(UIAction action, object arg)
    {

    }
}
