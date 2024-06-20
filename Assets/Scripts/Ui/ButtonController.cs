using System;
using UnityEngine.UI;

[System.Serializable]
public class ButtonController
{
    public Button editButton;
    public Button applySetTimeButton;
    public Button dragEditButton;
    public Button applyDragButton;
    public Button closeButton;

    public void Initialize(Action onEditClick, Action onApplySetTimeClick, Action onDragEditClick, Action onApplyDragClick, Action onCloseClick)
    {
        editButton.onClick.AddListener(() => onEditClick?.Invoke());
        applySetTimeButton.onClick.AddListener(() => onApplySetTimeClick?.Invoke());
        dragEditButton.onClick.AddListener(() => onDragEditClick?.Invoke());
        applyDragButton.onClick.AddListener(() => onApplyDragClick?.Invoke());
        closeButton.onClick.AddListener(() => onCloseClick?.Invoke());

        SetApplyDragButtonInteractable(false);
    }

    public void SetApplyDragButtonInteractable(bool interactable)
    {
        applyDragButton.interactable = interactable;
    }
}
