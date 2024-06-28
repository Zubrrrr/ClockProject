using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EditInputModule : MonoBehaviour
{
    private const string ErrorMessageDisplay = "Invalid input for hours or minutes. Please enter numeric values.";
    private const string ManualTimeInputDisplayName = "Manual Time Input";

    [SerializeField] private GameObject _editPanel;
    [SerializeField] private Button _applaySetTimeButton;
    [SerializeField] private TextMeshProUGUI _errorTextUI;
    [SerializeField] private TextMeshProUGUI _timezoneTextUI;

    [SerializeField] private TMP_InputField _hourInput;
    [SerializeField] private TMP_InputField _minuteInput;

    [SerializeField] private UiDropdown _dropdown;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnApiInitialized += Initialize;
    }

    private void Initialize()
    {
        _applaySetTimeButton.onClick.AddListener(OnSetTimeButton);
        ClearErrorText();
    }

    private void OnSetTimeButton()
    {
        if (int.TryParse(_hourInput.text, out int hours) && int.TryParse(_minuteInput.text, out int minutes))
        {
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                SetErrorText(ErrorMessageDisplay);
                return;
            }

            DateTime newTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
            _timeService.SetCurrentTime(newTime);
            _timeService.ResumeTime();
            _timezoneTextUI.text = ManualTimeInputDisplayName;
            _hourInput.text = "";
            _minuteInput.text = "";
            ClearErrorText();
            _editPanel.SetActive(false);

            _dropdown.AddManualTimeOptionToDropdown();
        }
        else
        {
            SetErrorText(ErrorMessageDisplay);
        }
    }

    private void ClearErrorText() => _errorTextUI.text = "";

    private void SetErrorText(string errorMessage) => _errorTextUI.text = errorMessage;
}
