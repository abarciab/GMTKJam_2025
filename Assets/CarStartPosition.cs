using UnityEngine;

public class CarStartPosition : MonoBehaviour
{
    [SerializeField] private GameObject _helperModel;
    
    void Start()
    {
        Destroy(_helperModel);
    }

}
