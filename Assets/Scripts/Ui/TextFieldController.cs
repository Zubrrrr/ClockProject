using System;
using TMPro;

[System.Serializable]
public class TextFieldController
{
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timezoneText;

    public void Initialize(Action onHourChanged, Action onMinuteChanged)
    {
        hourInput.onValueChanged.AddListener(delegate { onHourChanged?.Invoke(); });
        minuteInput.onValueChanged.AddListener(delegate { onMinuteChanged?.Invoke(); });
    }

    public void ClearErrorText()
    {
        errorText.text = "";
    }

    public void SetErrorText(string errorMessage)
    {
        errorText.text = errorMessage;
    }

    public void UpdateTimeText(DateTime currentTime)
    {
        timeText.text = currentTime.ToString("HH:mm:ss");
        dateText.text = currentTime.ToString("yyyy-MM-dd");
    }

    public void UpdateTimezoneText(string timezone)
    {
        timezoneText.text = timezone;
    }

    public void UpdateRealtimeTimeText(DateTime currentTime)
    {
        timeText.text = currentTime.ToString("HH:mm:ss");
    }
}
