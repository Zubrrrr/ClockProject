using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class DropdownModule : MonoBehaviour
{
    [SerializeField] private UiDropdown _uiDropdown;

    private ITimeService _timeService;

    private readonly (string Id, string DisplayName)[] timezones = new (string, string)[]
    {
        ("Europe/Moscow", "Moscow (GMT+3)"),
        ("America/New_York", "New York (GMT-4)"),
        ("Asia/Tokyo", "Tokyo (GMT+9)"),
        ("Europe/London", "London (GMT+1)"),
        ("Australia/Sydney", "Sydney (GMT+10)")
    };

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnApiInitialized += Initialize;
    }

    private void Initialize()
    {
        _uiDropdown.TimezoneDropdown.onValueChanged.AddListener(OnTimezoneChanged);
        ClearDropdownOptions();
        SetDropdownOptions();
    }

    private void ClearDropdownOptions() => _uiDropdown.TimezoneDropdown.options.Clear();

    private void SetDropdownOptions()
    {
        List<string> timezoneOptions = timezones.Select(tz => tz.DisplayName).ToList();
        _uiDropdown.TimezoneDropdown.AddOptions(timezoneOptions);
        int timezoneIndex = Array.FindIndex(timezones, tz => tz.Id == _timeService.GetCurrentTimezone());
        if (timezoneIndex >= 0)
        {
            _uiDropdown.TimezoneDropdown.SetValueWithoutNotify(timezoneIndex);
        }
    }

    private void OnTimezoneChanged(int index)
    {
        if (index < 0 || index >= _uiDropdown.TimezoneDropdown.options.Count)
        {
            Debug.LogError($"Index {index} is out of range in OnTimezoneChanged.");
            return;
        }

        string selectedTimezone = _uiDropdown.TimezoneDropdown.options[index].text;

        int timezoneArrayIndex = Array.FindIndex(timezones, tz => tz.DisplayName == selectedTimezone);

        if (timezoneArrayIndex >= 0 && timezoneArrayIndex < timezones.Length)
        {
            _timeService.SetTimezone(timezones[timezoneArrayIndex].Id);
            _timeService.ResumeTime();
        }
    }
}