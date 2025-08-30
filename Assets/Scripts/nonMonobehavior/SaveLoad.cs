using MyBox;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveLoad
{
    private static string SaveFileName = "saveData.txt";
    private static string Path => Application.persistentDataPath + "/" + SaveFileName;

    private static string FormatKey(string key) => key.ToUpper();

    public static void SaveData<T>(string key, T data)
    {
        var type = typeof(T);

        if (type == typeof(Vector2)) {
            var vector2 = Utils.StringToVector2(data.ToString());

            SaveData(key + ".x", vector2.x);
            SaveData(key + ".y", vector2.y);
            return;
        }

        if (type == typeof(Vector3)) {
            var vector3 = Utils.StringToVector3(data.ToString());

            SaveData(key + ".x", vector3.x);
            SaveData(key + ".y", vector3.y);
            SaveData(key + ".z", vector3.z);
            return;
        }

        var allowedTypes = new List<System.Type>() { typeof(string), typeof(int), typeof(float), typeof(bool) };
        if (!allowedTypes.Contains(type)) Debug.LogError("unallowed type passed to Saveload.SaveData(): " + type);
        
        var entry = new string[] {FormatKey(key), data.ToString()};
        var alldata = GetAllSaveData();

        bool found = false;
        for (int i = 0; i < alldata.Count; i++) {
            if (alldata[i][0] == FormatKey(key)) {
                alldata[i][1] = data.ToString();
                found = true;
                break;
            }
        }
        if (!found) alldata.Add(entry);
          
        var saveLines = alldata.Select(x => x[0] + ":" +  x[1]).ToList();
        File.WriteAllLines(Path, saveLines);
    }

    public static string LoadData (string key, string defaultValue)
    {
        var data = GetAllSaveData();
        foreach (var item in data) {
            if (item[0] == FormatKey(key)) return item[1];
        }
        return defaultValue;
    }

    public static int LoadData(string key, int defaultValue)
    {
        var data = GetAllSaveData();
        foreach (var item in data) {
            try { if (item[0] == FormatKey(key)) return int.Parse(item[1]); }
            catch { continue; }
        }
        return defaultValue;
    }

    public static float LoadData(string key, float defaultValue)
    {
        var data = GetAllSaveData();
        foreach (var item in data) {
            try { if (item[0] == FormatKey(key)) return float.Parse(item[1]); }
            catch { continue; }
        }
        return defaultValue;
    }

    public static bool LoadData(string key, bool defaultValue)
    {
        var data = GetAllSaveData();
        foreach (var item in data) {
            try { if (item[0] == FormatKey(key)) return bool.Parse(item[1]); }
            catch { continue; }
        }
        return defaultValue;
    }

    public static Vector2 LoadData(string key, Vector2 defaultValue)
    {
        var x = LoadData(key + ".x", defaultValue.x);
        var y = LoadData(key + ".y", defaultValue.y);
        return new Vector2(x, y);
    }

    public static Vector3 LoadData(string key, Vector3 defaultValue)
    {
        var x = LoadData(key + ".x", defaultValue.x);
        var y = LoadData(key + ".y", defaultValue.y);
        var z = LoadData(key + ".z", defaultValue.z);
        return new Vector3(x, y, z);
    }

    private static List<string[]> GetAllSaveData()
    {
        if (!File.Exists(Path)) return new List<string[]>();

        var text = File.ReadAllText(Path);

        var data = new List<string[]>();
        var lines = text.Split("\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
        for (int i = 0; i < lines.Count; i++) {
            var parts = lines[i].Split(":");
            if (parts.Length != 2) continue;
            data.Add(parts);
        }

        return data;
        
    }
}
