using UnityEngine;

public enum SprintOptions { TOGGLE, HOLD}

public static class Settings
{
    private static string _mouseSaveString = "mouseSens";


    private static Vector2 _mouseSensitvitiy = new Vector2(1, 1);
    public static Vector2 MouseSensitivity { get { return _mouseSensitvitiy; }  set { SetSensitivity(value); } } //0-2

    public static void Load()
    {
        _mouseSensitvitiy = SaveLoad.LoadData(_mouseSaveString, new Vector2(1f, 1f));
    }

    public static void Save()
    {
        SaveLoad.SaveData(_mouseSaveString, _mouseSensitvitiy);
    }

    private static void SetSensitivity(Vector2 value)
    {
        _mouseSensitvitiy = value;
        Save();
    }
}
