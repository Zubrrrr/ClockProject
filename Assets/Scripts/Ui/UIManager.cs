using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ButtonController buttonController;
    [SerializeField] private TextFieldController textFieldController;
    [SerializeField] private DropdownController dropdownController;
    [SerializeField] private GameObject editPanel;

    private ITimeService _ItimeService;
    private IClock _Iclock;

    private readonly (string Id, string DisplayName)[] timezones = new (string, string)[]
    {
        ("Europe/Moscow", "Moscow (GMT+3)"),
        ("America/New_York", "New York (GMT-4)"),
        ("Asia/Tokyo", "Tokyo (GMT+9)"),
        ("Europe/London", "London (GMT+1)"),
        ("Australia/Sydney", "Sydney (GMT+10)")
    };

    private const string ManualTimeDisplayName = "Manual Time";
    private bool isManualTime = false;

    private void Start()
    {
        _ItimeService = TimeManager.Instance;
        _Iclock = FindObjectOfType<Clock>();

        if (_ItimeService == null || _Iclock == null)
        {
            Debug.LogError("TimeManager or Clock instance is not set or does not implement ITimeService.");
            return;
        }

        InitializeUI();
        SubscribeEvents();
        UpdateUI();
    }

    private void InitializeUI()
    {
        buttonController.Initialize(OnEditButtonClick, OnSetTimeButtonClick, OnDragEditButtonClick, OnApplyDragButtonClick, OnCloseButtonClick);
        textFieldController.Initialize(OnHourInputChanged, OnMinuteInputChanged);
        dropdownController.Initialize(OnTimezoneChanged);
        textFieldController.ClearErrorText();

        List<string> timezoneOptions = timezones.Select(tz => tz.DisplayName).ToList();
        dropdownController.SetOptions(timezoneOptions);

        int timezoneIndex = Array.FindIndex(timezones, tz => tz.Id == _ItimeService.GetCurrentTimezone());
        if (timezoneIndex >= 0)
        {
            dropdownController.SetSelectedIndex(timezoneIndex);
        }
        else
        {
            Debug.LogError("Current timezone not found in the dropdown options: " + _ItimeService.GetCurrentTimezone());
        }

        editPanel.SetActive(false);
    }

    private void SubscribeEvents()
    {
        _ItimeService.OnTimeUpdated += UpdateUI;
    }

    private void OnEditButtonClick()
    {
        editPanel.SetActive(true);
        buttonController.editButton.gameObject.SetActive(false);
        _ItimeService.PauseTime();
        textFieldController.UpdateTimezoneText("Edit Mode");
    }

    private void OnSetTimeButtonClick()
    {
        if (int.TryParse(textFieldController.hourInput.text, out int hours) && int.TryParse(textFieldController.minuteInput.text, out int minutes))
        {
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                textFieldController.SetErrorText("Invalid input for hours or minutes. Please enter numeric values.");
                return;
            }

            DateTime newTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
            _ItimeService.SetCurrentTime(newTime);
            isManualTime = true;

            if (!dropdownController.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
            {
                dropdownController.AddOption(ManualTimeDisplayName);
            }

            dropdownController.SetSelectedIndex(dropdownController.timezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName));
            textFieldController.UpdateTimezoneText(ManualTimeDisplayName);

            _ItimeService.ResumeTime();
            UpdateUI();
            CloseEditPanel();
        }
        else
        {
            textFieldController.SetErrorText("Invalid input for hours or minutes. Please enter numeric values.");
        }
    }

    private void OnDragEditButtonClick()
    {
        _ItimeService.PauseTime();
        _Iclock.PauseAnimation();
        _Iclock.StartDragEdit();
        buttonController.SetApplyDragButtonInteractable(true);
    }

    private void OnApplyDragButtonClick()
    {
        _Iclock.ApplyDragEdit();
        isManualTime = true;

        if (!dropdownController.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
        {
            dropdownController.AddOption(ManualTimeDisplayName);
        }

        dropdownController.SetSelectedIndex(dropdownController.timezoneDropdown.options.FindIndex(option => option.text == ManualTimeDisplayName));

        _ItimeService.ResumeTime();
        UpdateUI();
        CloseEditPanel();
    }

    private void OnCloseButtonClick()
    {
        isManualTime = false;
        editPanel.SetActive(false);
        string currentTimezone = _ItimeService.GetCurrentTimezone();
        textFieldController.UpdateTimezoneText(_ItimeService.GetCurrentTimezoneWithOffset());

        int timezoneIndex = Array.FindIndex(timezones, tz => tz.Id == currentTimezone);
        if (timezoneIndex >= 0)
        {
            dropdownController.SetSelectedIndex(timezoneIndex);
        }
        else
        {
            Debug.LogError("Current timezone not found in the dropdown options: " + currentTimezone);
        }

        StartCoroutine(_ItimeService.UpdateTimeFromServer());
        _ItimeService.ResumeTime();
        textFieldController.ClearErrorText();
        _Iclock.ResetDragEdit();
        CloseEditPanel();
    }

    private void OnHourInputChanged()
    {
        textFieldController.ClearErrorText();
    }

    private void OnMinuteInputChanged()
    {
        textFieldController.ClearErrorText();
    }

    private void OnTimezoneChanged(int index)
    {
        if (index < 0 || index >= dropdownController.timezoneDropdown.options.Count)
        {
            Debug.LogError($"Index {index} is out of range in OnTimezoneChanged.");
            return;
        }

        string selectedTimezone = dropdownController.timezoneDropdown.options[index].text;
        if (selectedTimezone == ManualTimeDisplayName)
        {
            isManualTime = true;
        }
        else
        {
            if (dropdownController.timezoneDropdown.options.Exists(option => option.text == ManualTimeDisplayName))
            {
                dropdownController.RemoveOption(ManualTimeDisplayName);
            }

            int timezoneArrayIndex = Array.FindIndex(timezones, tz => tz.DisplayName == selectedTimezone);
            if (timezoneArrayIndex >= 0 && timezoneArrayIndex < timezones.Length)
            {
                _ItimeService.SetTimezone(timezones[timezoneArrayIndex].Id);
            }
            else
            {
                Debug.LogError("Selected timezone not found in the timezones array.");
            }

            isManualTime = false;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        DateTime currentTime = _ItimeService.GetCurrentTime();
        textFieldController.UpdateTimeText(currentTime);
        textFieldController.UpdateTimezoneText(isManualTime ? ManualTimeDisplayName : _ItimeService.GetCurrentTimezoneWithOffset());
    }

    private void Update()
    {
        DateTime currentTime = _ItimeService.GetCurrentTime();
        textFieldController.UpdateRealtimeTimeText(currentTime);
    }

    private void CloseEditPanel()
    {
        buttonController.SetApplyDragButtonInteractable(false);
        editPanel.SetActive(false);
        buttonController.editButton.gameObject.SetActive(true);
    }
}

























