using System;
using System.Collections;

public interface ITimeService
{
    IEnumerator UpdateTimeFromServer();
    DateTime GetCurrentTime();
    void SetCurrentTime(DateTime newTime);
    string GetCurrentTimezone();
    string GetCurrentTimezoneWithOffset();
    void SetTimezone(string timezoneId);
    void PauseTime();
    void ResumeTime();
    event Action OnTimeUpdated;
    event Action OnPauseTime;
    event Action OnResumeTime;
}