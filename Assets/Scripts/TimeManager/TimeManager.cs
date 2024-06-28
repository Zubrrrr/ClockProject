using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour, ITimeService
{
    private const string apiUrlTemplate = "https://worldtimeapi.org/api/timezone/{0}";

    private string _currentTimeZoneId = "UTC";
    private TimeSpan _currentTimeOffset = TimeSpan.Zero;
    private DateTime _currentTime;
    private bool _isPaused = false;
    private bool _suppressTimeUpdated = false;

    public event Action<DateTime> OnTimeUpdated;
    public event Action<TimeEvent> OnTimeEvent;
    public event Action OnApiInitialized;

    private void Start()
    {
        SetTimezone("Europe/Moscow");
        StartCoroutine(UpdateTimeFromServer());
        InvokeRepeating(nameof(UpdateTimeFromServer), 3600f, 3600f);
    }

    private void Update() => UpdateTime();

    public void SetTimezone(string timezoneId)
    {
        _currentTimeZoneId = timezoneId;
        _currentTimeOffset = GetTimezoneOffset(timezoneId);
        StartCoroutine(UpdateTimeFromServer());
    }

    public string GetCurrentTimezone()
    {
        return _currentTimeZoneId;
    }

    public string GetCurrentTimezoneWithOffset()
    {
        string offsetSign = _currentTimeOffset.TotalHours >= 0 ? "+" : "-";
        return $"{_currentTimeZoneId.Replace('_', ' ')} (GMT{offsetSign}{Math.Abs(_currentTimeOffset.Hours):00}:{Math.Abs(_currentTimeOffset.Minutes):00})";
    }

    public void SetCurrentTime(DateTime newTime)
    {
        _currentTime = newTime;
        if (!_suppressTimeUpdated)
        {
            OnTimeUpdated?.Invoke(_currentTime);
        }
        OnTimeEvent?.Invoke(TimeEvent.TimeUpdated);
    }

    public DateTime GetCurrentTime()
    {
        return _currentTime;
    }

    public IEnumerator UpdateTimeFromServer()
    {
        string apiUrl = string.Format(apiUrlTemplate, _currentTimeZoneId);
        Debug.Log("Fetching time from: " + apiUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("API Response: " + jsonResponse);
                var timeData = JsonUtility.FromJson<TimeData>(jsonResponse);
                DateTime utcTime = DateTime.Parse(timeData.datetime).ToUniversalTime();
                _currentTime = utcTime + _currentTimeOffset;
                Debug.Log("Time updated to: " + _currentTime);

                if (!_suppressTimeUpdated)
                {
                    OnTimeUpdated?.Invoke(_currentTime);
                }
                OnTimeEvent?.Invoke(TimeEvent.TimeUpdated);
                OnApiInitialized?.Invoke(); // Вызов события
            }
            else
            {
                Debug.LogError("Error fetching time from server: " + request.error);
            }
        }
    }

    private void UpdateTime()
    {
        if (!_isPaused)
        {
            _currentTime = _currentTime.AddSeconds(Time.deltaTime);
            if (!_suppressTimeUpdated)
            {
                OnTimeUpdated?.Invoke(_currentTime);
            }
        }
    }

    public void PauseTime()
    {
        _isPaused = true;
        OnTimeEvent?.Invoke(TimeEvent.TimePaused);
    }

    public void ResumeTime()
    {
        _isPaused = false;
        OnTimeEvent?.Invoke(TimeEvent.TimeResumed);
    }

    private TimeSpan GetTimezoneOffset(string timezoneId)
    {
        switch (timezoneId)
        {
            case "Europe/Moscow": return new TimeSpan(3, 0, 0);
            case "America/New_York": return new TimeSpan(-4, 0, 0);
            case "Asia/Tokyo": return new TimeSpan(9, 0, 0);
            case "Europe/London": return new TimeSpan(1, 0, 0);
            case "Australia/Sydney": return new TimeSpan(10, 0, 0);
            default: return TimeSpan.Zero;
        }
    }

    [Serializable]
    private class TimeData
    {
        public string datetime;
    }
}