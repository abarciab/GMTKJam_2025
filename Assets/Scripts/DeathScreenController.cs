using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : UIController
{
    protected override void UpdateUI(UIAction action, object arg)
    {
        if (action == UIAction.SHOW_DEATH_SCREEN) gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Utils.SetCursor(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
