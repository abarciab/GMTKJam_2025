using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [ButtonMethod]
    public void MoveCar()
    {
        FindFirstObjectByType<Car>(FindObjectsInactive.Include).transform.position = transform.position;
    }
}
