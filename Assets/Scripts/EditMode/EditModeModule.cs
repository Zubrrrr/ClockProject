using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EditModeModule : MonoBehaviour
{
    private const string EditModeDisplayName = "EDIT MODE";
    private const string UnknownDisplayName = "Unknown";

    [SerializeField] private GameObject _editPanel;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private UiText _text ;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService timeService)
    {
        _timeService = timeService;
        _timeService.OnApiInitialized += Initialize;
    }

    private void Initialize()
    {
        _editButton.onClick.AddListener(OnEnterEditMode);
        _resetButton.onClick.AddListener(OnExitEditMode);
        _editPanel.SetActive(false);
    }

    private void OnEnterEditMode()
    {
        _editPanel.SetActive(true);
        _text.TimezoneTextUI.text = EditModeDisplayName;
        _text.DateTextUI.text = UnknownDisplayName;
        _text.TimeTextUI.text = UnknownDisplayName;
        _timeService.PauseTime();
    }

    private void OnExitEditMode()
    {
        _editPanel.SetActive(false);
        _timeService.ResumeTime();
        StartCoroutine(_timeService.UpdateTimeFromServer());
    }
}
