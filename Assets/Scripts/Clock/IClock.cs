using System;
using UnityEngine;

public interface IClock
{
    void StartDragEdit();
    void ApplyDragEdit();
    void ResetDragEdit();
    void PauseAnimation();
    void ResumeAnimation();
    void StartEditing();
    void StopEditing();
    void ChangeClockHandsColor(Color hourColor, Color minuteColor);
    DateTime CalculateTimeFromHands();
}