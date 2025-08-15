using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGate : MonoBehaviour
{
    private void Update()
    {
#if unityEditor
        if (Input.GetKeyDown(KeyCode.U)) GameManager.i.Transition();
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Car>() != null) {
            GameManager.i.CompleteArea();
        }
    }
}
