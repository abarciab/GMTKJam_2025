using UnityEngine;

public class TransitionScreenController : UIController
{
    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_TRANSITION) gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Continue()
    {
        GameManager.i.EnterNewArea();
    }
}
