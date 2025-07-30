using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ControlDefinition : MonoBehaviour
{
    [SerializeField] private List<string> _testList = new List<string>();

    [ButtonMethod]
    private void WriteToFile()
    {
        string filePath = "Assets/scripts/nonMonobehavior/ControlEnums.cs"; 

        var uppercaseList = _testList.Select(x => x.ToUpper()).ToList();
        StreamWriter writer = new StreamWriter(filePath);
        var line = "public enum Control {" + string.Join(", ", uppercaseList) + "}";

        writer.WriteLine(line);
        writer.Close();

        AssetDatabase.Refresh();

        Debug.Log("File written successfully.");

    }
}
