using UnityEngine;
using DG.Tweening;
using AnimationSystem;

public class RotateAnimation : BaseAnimation
{
    [Header("Rotation Settings")]

    [Tooltip("Whether to use the object's current rotation as the starting rotation for the animation.")]
    [SerializeField] private bool _useCurrentRotationAsStart;

    [Tooltip("The initial rotation of the object for the animation if not using the current rotation.")]
    [SerializeField] private Vector3 _initialRotation;

    [Tooltip("The target rotation the object will animate to.")]
    [SerializeField] private Vector3 _targetRotation;

    public override void PlayAnimation()
    {
        Vector3 startRotation = _useCurrentRotationAsStart ? transform.eulerAngles : _initialRotation;

        transform.eulerAngles = startRotation;
        CurrentTween = transform.DORotate(_targetRotation, Duration)
            .SetEase(EaseType)
            .SetLoops(Loops, LoopType)
            .SetDelay(Delay);
    }

    public override void StopAnimation()
    {
        base.StopAnimation();
    }
}