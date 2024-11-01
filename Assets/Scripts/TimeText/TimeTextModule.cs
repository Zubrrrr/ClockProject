using UnityEngine;
using Zenject;
using System;

public class TimeTextModule : MonoBehaviour
{
    [SerializeField] private UiText _texts;

    private EventAggregator _eventAggregator;

    [Inject]
    private void Construct(EventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    private void OnEnable()
    {
        _eventAggregator.Subscribe<TimeManager.TimeUpdatedEvent>(UpdateRealtimeTimeText);
    }

    private void OnDisable()
    {
        _eventAggregator.Unsubscribe<TimeManager.TimeUpdatedEvent>(UpdateRealtimeTimeText);
    }

    private void UpdateRealtimeTimeText(TimeManager.TimeUpdatedEvent eventData)
    {
        UpdateTimeTexts(eventData.CurrentTime);
    }

    private void UpdateTimeTexts(DateTime currentTime)
    {
        string formattedTime = currentTime.ToString("HH:mm:ss");

        foreach (TMPro.TextMeshProUGUI timeTextUI in _texts.TimeTextUIs)
        {
            if (timeTextUI != null)
            {
                timeTextUI.text = formattedTime;
            }
        }
        GetDateText(currentTime);
    }

    private void GetDateText(DateTime currentDate)
    {
        if (_texts.DateTextUI != null)
        {
            _texts.DateTextUI.text = currentDate.ToString("yyyy-MM-dd");
        }
    }
}