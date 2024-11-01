using UnityEngine;
using DG.Tweening;

namespace AnimationSystem
{
    public abstract class BaseAnimation : MonoBehaviour
    {
        protected float Duration => _duration;
        protected Ease EaseType => _easeType;
        protected int Loops => _loops;
        protected LoopType LoopType => _loopType;
        protected float Delay => _delay;
        protected Tween CurrentTween;

        [Header("General animation settings")]

        [Tooltip("Duration of the animation in seconds")]
        [SerializeField] private float _duration;

        [Tooltip("Easing type applied to the animation")]
        [SerializeField] private Ease _easeType;

        [Tooltip("Number of times the animation repeats. Set to 0 for infinite loops.")]
        [SerializeField] private int _loops;

        [Tooltip("Loop behavior for the animation (e.g., restart or ping-pong)")]
        [SerializeField] private LoopType _loopType;

        [Tooltip("Delay before the animation starts, in seconds")]
        [SerializeField] private float _delay;

        [Header("Startup Settings")]
        [Tooltip("Event that triggers the animation to play automatically")]
        [SerializeField] private AnimationEvent _triggerEvent;

        private void Start()
        {
            if (_triggerEvent == AnimationEvent.OnStart)
            {
                PlayAnimation();
            }
        }

        [ContextMenu("Preview Animation")]
        public void PreviewAnimation()
        {
            StopAnimation();
            PlayAnimation();
        }

        public abstract void PlayAnimation();

        public virtual void StopAnimation()
        {
            if (CurrentTween != null && CurrentTween.IsActive())
            {
                CurrentTween.Kill();
                CurrentTween = null;
            }
        }

        private void OnEnable()
        {
            if (_triggerEvent != AnimationEvent.None && _triggerEvent != AnimationEvent.OnStart)
            {
                EventSystem.Instance.RegisterEvent(_triggerEvent, PlayAnimation);
            }
        }

        private void OnDisable()
        {
            if (_triggerEvent != AnimationEvent.None && _triggerEvent != AnimationEvent.OnStart)
            {
                EventSystem.Instance.UnregisterEvent(_triggerEvent, PlayAnimation);
            }
        }
    }
}