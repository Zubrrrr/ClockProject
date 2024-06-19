using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIElements
{
    public GameObject editPanel;
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TMP_Dropdown timezoneDropdown;
    public Button editButton;
    public Button applySetTime;
    public Button dragEditButton;
    public Button applyDragButton;
    public Button closeButton;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timezoneText;
    public TextMeshProUGUI errorText;
}
