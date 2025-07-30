using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) GetComponent<Slider>().value -= 0.2f;
    }
}
