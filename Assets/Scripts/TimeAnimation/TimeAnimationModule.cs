using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TimeAnimationModule : MonoBehaviour
{
    [SerializeField] private List<ColorAnimation> _colorAnimations;

    private EventAggregator _eventAggregator;

    [Inject]
    public void Construct(EventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    private void OnEnable()
    {
        _eventAggregator.Subscribe<TimeManager.TimeUpdatedEvent>(OnTimeUpdated);
    }

    private void OnDisable()
    {
        _eventAggregator.Unsubscribe<TimeManager.TimeUpdatedEvent>(OnTimeUpdated);
    }

    private void OnTimeUpdated(TimeManager.TimeUpdatedEvent timeEvent)
    {
        int lastSecond = -1;
        int currentSecond = timeEvent.CurrentTime.Second;

        if (currentSecond == 0 && lastSecond != currentSecond)
        {
            foreach (ColorAnimation animation in _colorAnimations)
            {
                if (animation != null)
                {
                    animation.PlayAnimation();
                }
            }
        }
    }
}