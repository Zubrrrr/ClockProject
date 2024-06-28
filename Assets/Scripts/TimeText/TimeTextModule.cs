using System;
using UnityEngine;
using Zenject;

public class TimeTextModule : MonoBehaviour
{
    [SerializeField] private UiText _texts;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnTimeUpdated += UpdateRealtimeTimeText;
        _timeService.OnApiInitialized += Initialize;
    }

    private void Initialize()
    {
        UpdateTimezoneText(_timeService.GetCurrentTimezoneWithOffset());
        GetDateText(_timeService.GetCurrentTime());
    }

    private void UpdateTimeText(DateTime currentTime) => _texts.TimeTextUI.text = currentTime.ToString("HH:mm:ss");

    private void GetDateText(DateTime currentDate) => _texts.DateTextUI.text = currentDate.ToString("yyyy-MM-dd");

    private void UpdateTimezoneText(string timezone) => _texts.TimezoneTextUI.text = timezone;

    private void UpdateRealtimeTimeText(DateTime currentTime) => UpdateTimeText(currentTime);
}
