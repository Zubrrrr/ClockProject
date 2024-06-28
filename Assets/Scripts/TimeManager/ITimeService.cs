using System;
using System.Collections;

public interface ITimeService
{
    event Action<DateTime> OnTimeUpdated;
    event Action<TimeEvent> OnTimeEvent;
    event Action OnApiInitialized;

    DateTime GetCurrentTime();
    IEnumerator UpdateTimeFromServer();
    void SetCurrentTime(DateTime newTime);
    void SetTimezone(string timezoneId);
    string GetCurrentTimezone();
    string GetCurrentTimezoneWithOffset();
    void PauseTime();
    void ResumeTime();
}