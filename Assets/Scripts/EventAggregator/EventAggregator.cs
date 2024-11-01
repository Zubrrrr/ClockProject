using System;
using System.Collections.Generic;

public class EventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        Type eventType = typeof(TEvent);

        if (_subscribers.ContainsKey(eventType) == false)
        {
            _subscribers[eventType] = new List<Delegate>();
        }
        _subscribers[eventType].Add(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        Type eventType = typeof(TEvent);

        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].Remove(handler);

            if (_subscribers[eventType].Count == 0)
            {
                _subscribers.Remove(eventType);
            }
        }
    }

    public void Publish<TEvent>(TEvent eventData)
    {
        Type eventType = typeof(TEvent);

        if (_subscribers.ContainsKey(eventType))
        {
            List<Delegate> subscribersCopy = new List<Delegate>(_subscribers[eventType]);

            foreach (Delegate subscriber in subscribersCopy)
            {
                ((Action<TEvent>)subscriber)?.Invoke(eventData);
            }
        }
    }
}