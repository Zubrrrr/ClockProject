using System;
using System.Collections;

public interface ITimeService
{
    DateTime GetCurrentTime();
    IEnumerator UpdateTimeFromServer();
    void SetCurrentTime(DateTime newTime);
    void SetTimezone(string timezoneId);
    string GetCurrentTimezone();
    string GetCurrentTimezoneWithOffset();
}