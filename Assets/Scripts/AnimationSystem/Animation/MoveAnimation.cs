using UnityEngine;
using DG.Tweening;
using AnimationSystem;

public class MoveAnimation : BaseAnimation
{
    [Header("Movement Settings")]

    [Tooltip("Whether to use the object's current position as the starting position for the animation.")]
    [SerializeField] private bool _useCurrentPositionAsStart;

    [Tooltip("The initial position of the object for the animation if not using the current position.")]
    [SerializeField] private Vector3 _initialPosition;

    [Tooltip("The target position the object will animate to.")]
    [SerializeField] private Vector3 _targetPosition;

    public override void PlayAnimation()
    {
        Vector3 startPosition = _useCurrentPositionAsStart ? transform.position : _initialPosition;

        transform.position = startPosition;
        CurrentTween = transform.DOMove(_targetPosition, Duration)
            .SetEase(EaseType)
            .SetLoops(Loops, LoopType)
            .SetDelay(Delay);
    }
}