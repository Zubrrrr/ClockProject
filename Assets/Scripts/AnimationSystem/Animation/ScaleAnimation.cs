using UnityEngine;
using DG.Tweening;
using AnimationSystem;

public class ScaleAnimation : BaseAnimation
{
    [Header("Zoom Settings")]

    [Tooltip("Whether to use the object's current scale as the starting scale for the animation.")]
    [SerializeField] private bool _useCurrentScaleAsStart;

    [Tooltip("The initial scale of the object for the animation if not using the current scale.")]
    [SerializeField] private Vector3 _initialScale;

    [Tooltip("The target scale the object will animate to.")]
    [SerializeField] private Vector3 _targetScale;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void PlayAnimation()
    {
        StopAnimation();

        Vector3 startScale = _useCurrentScaleAsStart
            ? (_rectTransform != null ? _rectTransform.localScale : transform.localScale)
            : _initialScale;

        if (_rectTransform != null)
        {
            _rectTransform.localScale = startScale;
            CurrentTween = _rectTransform.DOScale(_targetScale, Duration)
                .SetEase(EaseType)
                .SetLoops(Loops, LoopType)
                .SetDelay(Delay);
        }
        else
        {
            transform.localScale = startScale;
            CurrentTween = transform.DOScale(_targetScale, Duration)
                .SetEase(EaseType)
                .SetLoops(Loops, LoopType)
                .SetDelay(Delay);
        }
    }

    public override void StopAnimation()
    {
        base.StopAnimation();
    }
}