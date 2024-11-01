using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class TimeManager : MonoBehaviour, ITimeService
{
    private string _currentTimeZoneId;

    private TimeSpan _currentTimeOffset;
    private TimeSpan _serverTimeOffset;

    private Coroutine _syncCoroutine;
    private EventAggregator _eventAggregator;

    [Inject]
    public void Construct(EventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    private void Start()
    {
        SetTimezone("Europe/Moscow");
        _syncCoroutine = StartCoroutine(SyncTimePeriodically());
    }

    private void Update()
    {
        InvokeTimeUpdated();
    }

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
        _serverTimeOffset = newTime.ToUniversalTime() - DateTime.UtcNow;

        DateTime currentTime = GetCurrentTime();

        _eventAggregator.Publish(new TimeUpdatedEvent { CurrentTime = currentTime });
    }

    public DateTime GetCurrentTime()
    {
        return DateTime.UtcNow + _serverTimeOffset + _currentTimeOffset;
    }

    public IEnumerator UpdateTimeFromServer()
    {
        const string ApiUrlTemplate = "https://timeapi.io/api/Time/current/zone?timeZone={0}";

        string apiUrl = string.Format(ApiUrlTemplate, _currentTimeZoneId);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResponse);

                DateTime serverTime = DateTime.Parse(timeData.dateTime);
                SetCurrentTime(serverTime);
            }
            else
            {
                Debug.LogError("Error fetching time from server: " + request.error);
            }
        }
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

    private void InvokeTimeUpdated()
    {
        DateTime currentTime = GetCurrentTime();
        _eventAggregator.Publish(new TimeUpdatedEvent { CurrentTime = currentTime });
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(InvokeTimeUpdated));

        if (_syncCoroutine != null)
        {
            StopCoroutine(_syncCoroutine);
        }
    }

    private IEnumerator SyncTimePeriodically()
    {
        float syncInterval = 3600f;
        bool isRunning = true;

        while (isRunning)
        {
            yield return UpdateTimeFromServer();
            yield return new WaitForSeconds(syncInterval);
        }
    }

    [Serializable]
    private class TimeData
    {
        public string dateTime;
    }

    public class TimeUpdatedEvent
    {
        public DateTime CurrentTime;
    }
}
