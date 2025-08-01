using UnityEngine;

public class TransitionScreenController : UIController
{
    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_TRANSITION) gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Utils.SetCursor(true); 
    }

    public void Continue()
    {
        Utils.SetCursor(false);
        gameObject.SetActive(false);
        GameManager.i.EnterNewArea();
    }
}
