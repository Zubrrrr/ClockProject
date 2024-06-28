using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ClockAnimationModule : MonoBehaviour
{
    [SerializeField] ClockHand _clockHand;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnTimeUpdated += UpdateClock;
    }

    private void Start()
    {
        InitializeClock();
        SubscribeToTimeServiceEvents();
    }

    private void SubscribeToTimeServiceEvents() => _timeService.OnTimeEvent += HandleTimeEvent;
    private void UnsubscribeFromTimeServiceEvents() => _timeService.OnTimeEvent -= HandleTimeEvent;
    private void OnDestroy() => UnsubscribeFromTimeServiceEvents();

    private void InitializeClock()
    {
        DateTime currentTime = _timeService.GetCurrentTime();

        float hours = currentTime.Hour % 12 + currentTime.Minute / 60f;
        float minutes = currentTime.Minute + currentTime.Second / 60f;
        float seconds = currentTime.Second + currentTime.Millisecond / 1000f;

        _clockHand.HourHandBone.localRotation = Quaternion.Euler(0, 0, -hours * 30f);
        _clockHand.MinuteHandBone.localRotation = Quaternion.Euler(0, 0, -minutes * 6f);
        _clockHand.SecondHandBone.localRotation = Quaternion.Euler(0, 0, -seconds * 6f);
    }

    private void HandleTimeEvent(TimeEvent timeEvent)
    {
        switch (timeEvent)
        {
            case TimeEvent.TimePaused:
                PauseAnimation();
                break;
            case TimeEvent.TimeResumed:
                ResumeAnimation();
                break;
            case TimeEvent.TimeUpdated:
                DateTime currentTime = _timeService.GetCurrentTime();
                UpdateClock(currentTime);
                break;
        }
    }

    private void PauseAnimation()
    {
        _clockHand.HourHandBone.DOKill();
        _clockHand.MinuteHandBone.DOKill();
        _clockHand.SecondHandBone.DOKill();
    }

    private void ResumeAnimation()
    {
        DateTime currentTime = _timeService.GetCurrentTime();
        UpdateClock(currentTime);
    }

    private void UpdateClock(DateTime time)
    {
        float hours = time.Hour % 12 + time.Minute / 60f;
        float minutes = time.Minute + time.Second / 60f;
        float seconds = time.Second + time.Millisecond / 1000f;

        float hourRotation = -hours * 30f;
        float minuteRotation = -minutes * 6f;
        float secondRotation = -seconds * 6f;

        AnimateHand(_clockHand.HourHandBone, hourRotation, 1f);
        AnimateHand(_clockHand.MinuteHandBone, minuteRotation, 1f);
        AnimateHand(_clockHand.SecondHandBone, secondRotation, 1f);
    }

    private void AnimateHand(Transform hand, float targetRotation, float duration)
    {
        hand.DOKill();
        hand.DOLocalRotate(new Vector3(0, 0, targetRotation), duration, RotateMode.Fast).SetEase(Ease.Linear);
    }
}