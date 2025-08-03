using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Car>() != null) {
            print("triggered end");
            GameManager.i.Transition();
        }
    }
}
