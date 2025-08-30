using MyBox;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [Button]
    public void UpdateDropdown()
    {
        GetComponent<DropdownOption>().Initialize(UIAction.SHOW_STATUS);
    }

    [Button]
    private void PrintTest()
    {
        print(Application.persistentDataPath);

        var testVector = new Vector2(1.5f, 3.5f);
        var parsed = Utils.StringToVector2(testVector.ToString());
        print("original: " + testVector + "\nparsed: " + parsed);
        
    }
}
