using System;
using System.Collections.Generic;

namespace AnimationSystem
{
    public class EventSystem
    {
        private static EventSystem _instance;
        public static EventSystem Instance => _instance ??= new EventSystem();

        private readonly Dictionary<AnimationEvent, Action> _eventDictionary = new Dictionary<AnimationEvent, Action>();

        private EventSystem() { }

        public void RegisterEvent(AnimationEvent animationEvent, Action callback)
        {
            if (_eventDictionary.TryGetValue(animationEvent, out var thisEvent))
            {
                thisEvent += callback;
                _eventDictionary[animationEvent] = thisEvent;
            }
            else
            {
                _eventDictionary.Add(animationEvent, callback);
            }
        }

        public void UnregisterEvent(AnimationEvent animationEvent, Action callback)
        {
            if (_eventDictionary.TryGetValue(animationEvent, out var thisEvent))
            {
                thisEvent -= callback;
                if (thisEvent == null)
                {
                    _eventDictionary.Remove(animationEvent);
                }
                else
                {
                    _eventDictionary[animationEvent] = thisEvent;
                }
            }
        }

        public void TriggerEvent(AnimationEvent animationEvent)
        {
            if (_eventDictionary.TryGetValue(animationEvent, out var thisEvent))
            {
                thisEvent.Invoke();
            }
        }
    }
}
