using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DropdownController
{
    public TMP_Dropdown timezoneDropdown;

    public void Initialize(Action<int> onTimezoneChanged)
    {
        timezoneDropdown.onValueChanged.AddListener(delegate (int value) { onTimezoneChanged?.Invoke(value); });
    }

    public void SetOptions(List<string> options)
    {
        timezoneDropdown.options.Clear();
        foreach (var option in options)
        {
            timezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = option });
        }
        timezoneDropdown.RefreshShownValue();
    }

    public int GetSelectedIndex()
    {
        return timezoneDropdown.value;
    }

    public void SetSelectedIndex(int index)
    {
        if (index >= 0 && index < timezoneDropdown.options.Count)
        {
            timezoneDropdown.value = index;
        }
        else
        {
            Debug.LogError($"Index {index} is out of range in SetSelectedIndex.");
        }
    }

    public void AddOption(string option)
    {
        timezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = option });
        timezoneDropdown.RefreshShownValue();
    }

    public void RemoveOption(string option)
    {
        int index = timezoneDropdown.options.FindIndex(o => o.text == option);
        if (index >= 0)
        {
            timezoneDropdown.options.RemoveAt(index);
            timezoneDropdown.RefreshShownValue();
        }
    }
}

