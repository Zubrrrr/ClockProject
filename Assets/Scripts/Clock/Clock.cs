using System;
using UnityEngine;
using DG.Tweening;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform _hourHandBone;
    [SerializeField] private Transform _minuteHandBone;
    [SerializeField] private Transform _secondHandBone;

    [SerializeField] private SpriteRenderer _hourHandColor;
    [SerializeField] private SpriteRenderer _minuteHandColor;

    private Transform _selectedHand; 
    private Camera _camera;

    private DateTime _lastUpdateTime;

    private bool _isPaused = false;
    private bool _isEditing = false; 

    private float _initialHandAngle; 
    private float _initialMouseAngle;

    private void Start()
    {
        InitializeClock();
        SubscribeToTimeManagerEvents();
        InitializeCamera();
    }

    private void OnDestroy()
    {
        UnsubscribeFromTimeManagerEvents();
    }

    private void Update()
    {
        if (_isEditing)
        {
            HandleMouseInput();
        }
        else if (!_isPaused) // Обновление стрелок только если не на паузе
        {
            DateTime currentTime = TimeManager.Instance.GetCurrentTime();
            if (currentTime != _lastUpdateTime)
            {
                UpdateClock(currentTime);
                _lastUpdateTime = currentTime;
            }
        }
    }

    public DateTime CalculateTimeFromHands()
    {
        float hourAngle = _hourHandBone.localEulerAngles.z;
        float minuteAngle = _minuteHandBone.localEulerAngles.z;

        int hours = Mathf.RoundToInt((360 - hourAngle) / 30f) % 12;
        int minutes = Mathf.RoundToInt((360 - minuteAngle) / 6f) % 60;

        if (hours == 0) hours = 12;

        DateTime currentTime = TimeManager.Instance.GetCurrentTime();
        return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hours, minutes, 0);
    }

    public void ChangeClockHandsColor(Color hourColor, Color minuteColor)
    {
        _hourHandColor.color = hourColor;
        _minuteHandColor.color = minuteColor;
    }

    public void PauseAnimation()
    {
        _isPaused = true;
        _hourHandBone.DOKill();
        _minuteHandBone.DOKill();
        _secondHandBone.DOKill();
    }

    public void ResumeAnimation()
    {
        _isPaused = false;
        DateTime currentTime = TimeManager.Instance.GetCurrentTime();
        UpdateClock(currentTime);
    }

    public void StartEditing()
    {
        _isEditing = true;
    }

    public void StopEditing()
    {
        _isEditing = false;
    }

    private void InitializeClock()
    {
        DateTime currentTime = TimeManager.Instance.GetCurrentTime();
        _lastUpdateTime = currentTime;

        float hours = currentTime.Hour % 12 + currentTime.Minute / 60f;
        float minutes = currentTime.Minute + currentTime.Second / 60f;
        float seconds = currentTime.Second + currentTime.Millisecond / 1000f;

        _hourHandBone.localRotation = Quaternion.Euler(0, 0, -hours * 30f);
        _minuteHandBone.localRotation = Quaternion.Euler(0, 0, -minutes * 6f);
        _secondHandBone.localRotation = Quaternion.Euler(0, 0, -seconds * 6f);
    }

    private void SubscribeToTimeManagerEvents()
    {
        TimeManager.Instance.OnPauseTime += PauseAnimation;
        TimeManager.Instance.OnResumeTime += ResumeAnimation;
    }

    private void UnsubscribeFromTimeManagerEvents()
    {
        TimeManager.Instance.OnPauseTime -= PauseAnimation;
        TimeManager.Instance.OnResumeTime -= ResumeAnimation;
    }

    private void InitializeCamera()
    {
        _camera = Camera.main;
    }

    private void UpdateClock(DateTime time)
    {
        float hours = time.Hour % 12 + time.Minute / 60f;
        float minutes = time.Minute + time.Second / 60f;
        float seconds = time.Second + time.Millisecond / 1000f;

        float hourRotation = -hours * 30f;
        float minuteRotation = -minutes * 6f;
        float secondRotation = -seconds * 6f;

        AnimateHand(_hourHandBone, hourRotation, 1f);
        AnimateHand(_minuteHandBone, minuteRotation, 1f);
        AnimateHand(_secondHandBone, secondRotation, 1f);
    }

    private void AnimateHand(Transform hand, float targetRotation, float duration)
    {
        hand.DOKill();
        hand.DOLocalRotate(new Vector3(0, 0, targetRotation), duration, RotateMode.Fast).SetEase(Ease.Linear);
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectedHand = GetSelectedHand();
            if (_selectedHand != null)
            {
                _initialHandAngle = _selectedHand.localEulerAngles.z;
                _initialMouseAngle = GetMouseAngle();
            }
        }

        if (Input.GetMouseButton(0) && _selectedHand != null)
        {
            RotateHandToMouse(_selectedHand, _initialHandAngle, _initialMouseAngle);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _selectedHand = null;
        }
    }

    private Transform GetSelectedHand()
    {
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.transform == _hourHandBone)
            {
                return _hourHandBone;
            }
            else if (hit.collider.transform == _minuteHandBone)
            {
                return _minuteHandBone;
            }
        }

        return null;
    }

    private void RotateHandToMouse(Transform hand, float initialHandAngle, float initialMouseAngle)
    {
        float currentMouseAngle = GetMouseAngle();
        float angleDelta = currentMouseAngle - initialMouseAngle;
        float newHandAngle = initialHandAngle + angleDelta;
        hand.localRotation = Quaternion.Euler(0, 0, newHandAngle);
    }

    private float GetMouseAngle()
    {
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}



