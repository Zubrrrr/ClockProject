using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ClockAnimationModule : MonoBehaviour
{
    [SerializeField]
    private List<ClockHandEntry> _clockHands;

    private EventAggregator _eventAggregator;

    [Inject]
    private void Construct(EventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    private void OnEnable()
    {
        _eventAggregator.Subscribe<TimeManager.TimeUpdatedEvent>(OnTimeUpdated);
    }

    private void OnDisable()
    {
        _eventAggregator.Unsubscribe<TimeManager.TimeUpdatedEvent>(OnTimeUpdated);
    }

    private void OnTimeUpdated(TimeManager.TimeUpdatedEvent eventData)
    {
        UpdateClock(eventData.CurrentTime);
    }

    private void UpdateClock(DateTime time)
    {
        foreach (ClockHandEntry hand in _clockHands)
        {
            float units = GetCurrentUnits(time, hand);
            float rotation = -units * hand.DegreesPerUnit;
            hand.HandBone.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    private void Start()
    {
        DateTime currentTime = DateTime.UtcNow;

        UpdateClock(currentTime);
    }

    private float GetCurrentUnits(DateTime time, ClockHandEntry hand)
    {
        int hour = 12;

        float minute = 60;
        float second = 60;
        float mikkisecond = 1000f;
        float units = 0f;

        switch (hand.TimeUnit)
        {
            case TimeUnit.Hour:
                units = time.Hour % hour;
                if (hand.IncludeNextLowerUnit)
                    units += time.Minute / minute;
                break;
            case TimeUnit.Minute:
                units = time.Minute;
                if (hand.IncludeNextLowerUnit)
                    units += time.Second / second;
                break;
            case TimeUnit.Second:
                units = time.Second;
                if (hand.IncludeNextLowerUnit)
                    units += time.Millisecond / mikkisecond;
                break;
            case TimeUnit.Millisecond:
                units = time.Millisecond;
                break;
        }
        return units;
    }
}