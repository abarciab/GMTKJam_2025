using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DropdownOption : MonoBehaviour
{
    public TMP_Dropdown _dropdown;

    public void Initialize<T> (T startingParam)
    {
        var enumList = Utils.EnumToList<T>();
        var startingIndex = 0;
        for (int i = 0; i < enumList.Count; i++) {
            if (enumList[i].Equals(startingParam)) startingIndex = i;
        }

        var stringOptions = enumList.Select(x => x.ToString().Replace("_", " ").ToLower()).ToList();
        for (int i = 0; i < stringOptions.Count; i++) {
            var firstLetter = stringOptions[i][0].ToString().ToUpper();
            var restOfWord = stringOptions[i].Substring(1);
            stringOptions[i] = firstLetter + restOfWord;
        }

        var newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (var s in stringOptions) newOptions.Add(new TMP_Dropdown.OptionData(s));

        _dropdown.options = newOptions;
        _dropdown.SetValueWithoutNotify(startingIndex);
    }
}
