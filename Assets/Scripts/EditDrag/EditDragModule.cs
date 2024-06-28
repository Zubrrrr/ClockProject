using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EditDragModule : MonoBehaviour
{
    private const string ManualTimeDragDisplayName = "Manual Time Drag";

    [SerializeField] private Button _dragEditButton;
    [SerializeField] private Button _applyDragButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private TextMeshProUGUI _timezoneTextUI;

    [SerializeField] private GameObject _editPanel;
    [SerializeField] private SpriteRenderer _hourHandColor;
    [SerializeField] private SpriteRenderer _minuteHandColor;

    [SerializeField] private ClockHand _clockHand;
    [SerializeField] private UiDropdown _dropdown;

    private Transform _selectedHand;
    private Camera _camera;
    private bool _isStartDragEdit = false;

    private float _initialHandAngle;
    private float _initialMouseAngle;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnApiInitialized += Initialize;
    }

    private void Initialize()
    {
        _applyDragButton.onClick.AddListener(ApplyDragEdit);
        _dragEditButton.onClick.AddListener(StartDragEdit);
        _resetButton.onClick.AddListener(ResetDragEdit);
        InitializeCamera();
        SetApplyDragButtonInteractable(false);
    }

    private void Update()
    {
        if (_isStartDragEdit)
        {
            HandleMouseInput();
        }
    }

    private void StartDragEdit()
    {
        ChangeClockHandsColor(Color.blue, Color.green);
        _timeService.PauseTime();
        _isStartDragEdit = true;
        SetApplyDragButtonInteractable(true);
    }

    private void ApplyDragEdit()
    {
        DateTime newTime = CalculateTimeFromHands();
        _timeService.SetCurrentTime(newTime);
        ChangeClockHandsColor(Color.black, Color.black);
        _timeService.ResumeTime();
        _isStartDragEdit = false;
        _editPanel.SetActive(false);
        _dropdown.AddManualTimeOptionToDropdown();
        _timezoneTextUI.text = ManualTimeDragDisplayName;
    }

    private void ResetDragEdit() => ChangeClockHandsColor(Color.black, Color.black);

    private DateTime CalculateTimeFromHands()
    {
        float hourAngle = _clockHand.HourHandBone.localEulerAngles.z;
        float minuteAngle = _clockHand.MinuteHandBone.localEulerAngles.z;

        int hours = Mathf.RoundToInt((360 - hourAngle) / 30f) % 12;
        int minutes = Mathf.RoundToInt((360 - minuteAngle) / 6f) % 60;

        if (hours == 0) hours = 12;

        DateTime currentTime = _timeService.GetCurrentTime();
        return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hours, minutes, 0);
    }

    private void ChangeClockHandsColor(Color hourColor, Color minuteColor)
    {
        _hourHandColor.color = hourColor;
        _minuteHandColor.color = minuteColor;
    }

    private void InitializeCamera() => _camera = Camera.main;

    private void SetApplyDragButtonInteractable(bool interactable)
    {
        _applyDragButton.interactable = interactable;
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectedHand = GetSelectedHand();
            if (_selectedHand != null)
            {
                _initialHandAngle = _selectedHand.localEulerAngles.z;
                _initialMouseAngle = GetMouseAngle(_selectedHand);
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
            if (hit.collider.transform == _clockHand.HourHandBone)
            {
                return _clockHand.HourHandBone;
            }
            else if (hit.collider.transform == _clockHand.MinuteHandBone)
            {
                return _clockHand.MinuteHandBone;
            }
        }

        return null;
    }

    private void RotateHandToMouse(Transform hand, float initialHandAngle, float initialMouseAngle)
    {
        float currentMouseAngle = GetMouseAngle(hand);
        float angleDelta = currentMouseAngle - initialMouseAngle;
        float newHandAngle = initialHandAngle + angleDelta;
        hand.localRotation = Quaternion.Euler(0, 0, newHandAngle);
    }

    private float GetMouseAngle(Transform hand)
    {
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - hand.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}