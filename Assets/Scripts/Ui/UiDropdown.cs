using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class UiDropdown
{
    private const string ManualTimeDisplayName = "Manual Time";

    public TMP_Dropdown TimezoneDropdown;

    public void AddManualTimeOptionToDropdown()
    {
        if (!TimezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
        {
            TimezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = ManualTimeDisplayName });
            TimezoneDropdown.RefreshShownValue();
        }

        int manualTimeIndex = TimezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName);
        if (manualTimeIndex != -1)
        {
            TimezoneDropdown.value = manualTimeIndex;
            TimezoneDropdown.RefreshShownValue();
        }
    }
}