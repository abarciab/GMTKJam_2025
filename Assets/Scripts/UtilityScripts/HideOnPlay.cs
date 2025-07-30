using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Awake()
    {
        if (Time.timeSinceLevelLoad < 0.1f) gameObject.SetActive(false);
    }
}
