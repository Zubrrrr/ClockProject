using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private const string apiUrlTemplate = "https://worldtimeapi.org/api/timezone/{0}";

    private DateTime _currentTime;
    private string _currentTimeZoneId;
    private TimeSpan _currentTimeOffset;

    public event Action OnTimeUpdated;
    public event Action OnPauseTime;
    public event Action OnResumeTime;

    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TimeManager instance created.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetTimezone("Europe/Moscow");
        StartCoroutine(UpdateTimeFromServer());
        InvokeRepeating(nameof(UpdateTimeFromServer), 3600f, 3600f);
    }

    public IEnumerator UpdateTimeFromServer()
    {
        string apiUrl = string.Format(apiUrlTemplate, _currentTimeZoneId);
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("API Response: " + jsonResponse); // Логирование ответа для отладки
                var timeData = JsonUtility.FromJson<TimeData>(jsonResponse);
                DateTime utcTime = DateTime.Parse(timeData.datetime).ToUniversalTime();
                _currentTime = utcTime + _currentTimeOffset;
                Debug.Log("Time updated: " + _currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
                OnTimeUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Error fetching time from server: " + request.error);
            }
        }
    }

    private void Update()
    {
        if (!_isPaused) 
        {
            _currentTime = _currentTime.AddSeconds(Time.deltaTime);
        }
    }

    public DateTime GetCurrentTime()
    {
        return _currentTime;
    }

    public void SetCurrentTime(DateTime newTime)
    {
        _currentTime = newTime;
    }

    public string GetCurrentTimezone()
    {
        return _currentTimeZoneId ?? "UTC";
    }

    public string GetCurrentTimezoneWithOffset()
    {
        string offsetSign = _currentTimeOffset.TotalHours >= 0 ? "+" : "-";
        return $"{_currentTimeZoneId.Replace('_', ' ')} (GMT{offsetSign}{Math.Abs(_currentTimeOffset.Hours):00}:{Math.Abs(_currentTimeOffset.Minutes):00})";
    }

    public void SetTimezone(string timezoneId)
    {
        try
        {
            _currentTimeZoneId = timezoneId;
            _currentTimeOffset = GetTimezoneOffset(timezoneId);
            Debug.Log("Timezone set to: " + _currentTimeZoneId);
            StartCoroutine(UpdateTimeFromServer());
        }
        catch (Exception ex)
        {
            Debug.LogError("Error setting timezone: " + ex.Message);
        }
    }

    public void PauseTime()
    {
        _isPaused = true;
        OnPauseTime?.Invoke();
    }

    public void ResumeTime()
    {
        _isPaused = false;
        OnResumeTime?.Invoke();
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