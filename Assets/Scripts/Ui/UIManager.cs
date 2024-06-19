using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIElements uiElements;

    private const string ManualTimeDisplayName = "Manual Time";

    private bool _isManualTime = false;
    private bool _changesApplied = false;
    private bool _isDragEditing = false;

    private readonly (string Id, string DisplayName)[] _timezones = new (string, string)[]
    {
        ("Europe/Moscow", "Moscow (GMT+3)"),
        ("America/New_York", "New York (GMT-4)"),
        ("Asia/Tokyo", "Tokyo (GMT+9)"),
        ("Europe/London", "London (GMT+1)"),
        ("Australia/Sydney", "Sydney (GMT+10)")
    };

    private void Start()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager instance is not set.");
            return;
        }

        InitializeUI();
        InitializeTimezoneDropdown();
        SubscribeEvents();

        UpdateUI();
    }

    private void InitializeUI()
    {
        uiElements.editPanel.SetActive(false);
        uiElements.errorText.text = "";
        uiElements.applyDragButton.interactable = false;

        uiElements.applySetTime.onClick.AddListener(OnSetTimeButtonClick);
        uiElements.editButton.onClick.AddListener(OnEditButtonClick);
        uiElements.dragEditButton.onClick.AddListener(OnDragEditButtonClick);
        uiElements.applyDragButton.onClick.AddListener(OnApplyDragButtonClick);
        uiElements.closeButton.onClick.AddListener(OnCloseButtonClick);

        uiElements.hourInput.onValueChanged.AddListener(delegate { ClearErrorTextOnValidInput(); });
        uiElements.minuteInput.onValueChanged.AddListener(delegate { ClearErrorTextOnValidInput(); });
    }

    private void InitializeTimezoneDropdown()
    {
        uiElements.timezoneDropdown.options.Clear();

        foreach (var timezone in _timezones)
        {
            uiElements.timezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = timezone.DisplayName });
        }

        uiElements.timezoneDropdown.onValueChanged.AddListener(delegate { OnTimezoneChanged(); });

        string currentTimezone = TimeManager.Instance.GetCurrentTimezone();
        Debug.Log("Current Timezone: " + currentTimezone);
        int timezoneIndex = Array.FindIndex(_timezones, tz => tz.Id == currentTimezone);

        if (timezoneIndex >= 0)
        {
            uiElements.timezoneDropdown.value = timezoneIndex;
        }
        else
        {
            Debug.LogError("Current timezone not found in the dropdown options: " + currentTimezone);
        }
    }

    private void SubscribeEvents()
    {
        TimeManager.Instance.OnTimeUpdated += UpdateUI;
    }

    private void OnEditButtonClick()
    {
        uiElements.editPanel.SetActive(true);
        uiElements.editButton.gameObject.SetActive(false);
        TimeManager.Instance.PauseTime();
        uiElements.timezoneText.text = "Edit Mode";
    }

    private void OnDragEditButtonClick()
    {
        _isDragEditing = !_isDragEditing;
        var clock = FindObjectOfType<Clock>();

        if (_isDragEditing)
        {
            TimeManager.Instance.PauseTime();
            clock.PauseAnimation();
            clock.StartEditing();
            uiElements.applyDragButton.interactable = true;
            clock.ChangeClockHandsColor(Color.blue, Color.green);
        }
        else
        {
            TimeManager.Instance.ResumeTime();
            clock.ResumeAnimation();
            clock.StopEditing();
            _changesApplied = false;
            uiElements.applyDragButton.interactable = false;
            clock.ChangeClockHandsColor(Color.black, Color.black);
        }
    }

    private void OnApplyDragButtonClick()
    {
        var clock = FindObjectOfType<Clock>();
        clock.StopEditing();
        DateTime newTime = clock.CalculateTimeFromHands();
        TimeManager.Instance.SetCurrentTime(newTime);
        _isManualTime = true;
        _changesApplied = true;

        if (!uiElements.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
        {
            uiElements.timezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = ManualTimeDisplayName });
        }

        uiElements.timezoneDropdown.value = uiElements.timezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName);

        _isDragEditing = false;
        TimeManager.Instance.ResumeTime();
        uiElements.timezoneText.text = ManualTimeDisplayName;
        clock.ResumeAnimation();
        UpdateUI();
        CloseEditPanel();
        clock.ChangeClockHandsColor(Color.black, Color.black);
    }

    private void OnCloseButtonClick()
    {
        _isManualTime = false; 
        _changesApplied = false; 
        StartCoroutine(TimeManager.Instance.UpdateTimeFromServer());
        uiElements.editPanel.SetActive(false);
        string currentTimezone = TimeManager.Instance.GetCurrentTimezone();
        uiElements.timezoneText.text = TimeManager.Instance.GetCurrentTimezoneWithOffset();

        int timezoneIndex = Array.FindIndex(_timezones, tz => tz.Id == currentTimezone);
        if (timezoneIndex >= 0)
        {
            uiElements.timezoneDropdown.value = timezoneIndex;
        }
        else
        {
            Debug.LogError("Current timezone not found in the dropdown options: " + currentTimezone);
        }

        TimeManager.Instance.ResumeTime();
        ClearErrorText();
        CloseEditPanel();
        var clock = FindObjectOfType<Clock>();
        clock.ChangeClockHandsColor(Color.black, Color.black);
    }

    private void OnSetTimeButtonClick()
    {
        if (int.TryParse(uiElements.hourInput.text, out int hours) && int.TryParse(uiElements.minuteInput.text, out int minutes))
        {
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                uiElements.errorText.text = "Invalid input for hours or minutes. Please enter numeric values.";
                return;
            }

            DateTime newTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
            TimeManager.Instance.SetCurrentTime(newTime);
            _isManualTime = true;
            _changesApplied = true;

            if (!uiElements.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
            {
                uiElements.timezoneDropdown.options.Add(new TMP_Dropdown.OptionData { text = ManualTimeDisplayName });
            }

            uiElements.timezoneDropdown.value = uiElements.timezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName);

            uiElements.timezoneText.text = ManualTimeDisplayName;
            TimeManager.Instance.ResumeTime();
            UpdateUI();
            CloseEditPanel();
            var clock = FindObjectOfType<Clock>();
            clock.ChangeClockHandsColor(Color.black, Color.black);
            ClearErrorText();
        }
        else
        {
            uiElements.errorText.text = "Invalid input for hours or minutes. Please enter numeric values.";
        }
    }

    private void OnTimezoneChanged()
    {
        string selectedText = uiElements.timezoneDropdown.options[uiElements.timezoneDropdown.value].text;
        if (selectedText == ManualTimeDisplayName)
        {
            _isManualTime = true;
        }
        else
        {
            if (uiElements.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
            {
                int manualIndex = uiElements.timezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName);
                uiElements.timezoneDropdown.options.RemoveAt(manualIndex);
                uiElements.timezoneDropdown.RefreshShownValue();
            }

            string selectedTimezone = _timezones[uiElements.timezoneDropdown.value].Id;
            TimeManager.Instance.SetTimezone(selectedTimezone);
            _isManualTime = false;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        DateTime currentTime = TimeManager.Instance.GetCurrentTime();
        uiElements.timeText.text = currentTime.ToString("HH:mm:ss");
        uiElements.dateText.text = currentTime.ToString("yyyy-MM-dd");
        uiElements.timezoneText.text = _isManualTime ? ManualTimeDisplayName : TimeManager.Instance.GetCurrentTimezoneWithOffset();
    }

    private void Update()
    {
        DateTime currentTime = TimeManager.Instance.GetCurrentTime();
        uiElements.timeText.text = currentTime.ToString("HH:mm:ss");
        uiElements.dateText.text = currentTime.ToString("yyyy-MM-dd");
    }

    private void CloseEditPanel()
    {
        uiElements.editPanel.SetActive(false);
        uiElements.editButton.gameObject.SetActive(true);
        uiElements.applyDragButton.interactable = false;
    }

    private void ClearErrorTextOnValidInput()
    {
        if (int.TryParse(uiElements.hourInput.text, out int hours) && int.TryParse(uiElements.minuteInput.text, out int minutes))
        {
            if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
            {
                uiElements.errorText.text = "";
            }
        }
    }

    private void ClearErrorText()
    {
        uiElements.errorText.text = "";
    }
}

























