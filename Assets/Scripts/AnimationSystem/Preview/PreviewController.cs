using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationSystem
{
    public class PreviewController : MonoBehaviour
    {
        [Tooltip("List of animation collections to manage")]
        public List<CollectionPreview> AnimationCollections = new List<CollectionPreview>();

        private enum AnimationType { Color, Scale, Rotate, Move, Fade }

        private Dictionary<AnimationType, bool> _playAnimationTypes = new Dictionary<AnimationType, bool>()
        {
            { AnimationType.Color, true },
            { AnimationType.Scale, true },
            { AnimationType.Rotate, true },
            { AnimationType.Move, true },
            { AnimationType.Fade, true }
        };

        private static readonly Dictionary<AnimationType, Func<CollectionPreview, IEnumerable<BaseAnimation>>> AnimationTypeToAnimations = new Dictionary<AnimationType, Func<CollectionPreview, IEnumerable<BaseAnimation>>>
        {
            { AnimationType.Color, c => c.ColorAnimations != null ? c.ColorAnimations.Cast<BaseAnimation>() : Enumerable.Empty<BaseAnimation>() },
            { AnimationType.Scale, c => c.ScaleAnimations != null ? c.ScaleAnimations.Cast<BaseAnimation>() : Enumerable.Empty<BaseAnimation>() },
            { AnimationType.Rotate, c => c.RotateAnimations != null ? c.RotateAnimations.Cast<BaseAnimation>() : Enumerable.Empty<BaseAnimation>() },
            { AnimationType.Move, c => c.MoveAnimations != null ? c.MoveAnimations.Cast<BaseAnimation>() : Enumerable.Empty<BaseAnimation>() },
            { AnimationType.Fade, c => c.FadeAnimations != null ? c.FadeAnimations.Cast<BaseAnimation>() : Enumerable.Empty<BaseAnimation>() },
        };

        private void PerformActionOnSelectedAnimations(int index, Action<BaseAnimation> action)
        {
            if (index < 0 || index >= AnimationCollections.Count) return;

            CollectionPreview collection = AnimationCollections[index];

            foreach (AnimationType animationType in _playAnimationTypes.Keys)
            {
                if (_playAnimationTypes[animationType] == false) continue;

                Func<CollectionPreview, IEnumerable<BaseAnimation>> getAnimations = AnimationTypeToAnimations[animationType];
                IEnumerable<BaseAnimation> animations = getAnimations(collection);

                foreach (BaseAnimation animation in animations)
                {
                    if (animation != null)
                    {
                        action(animation);
                    }
                }
            }
        }

        public void PlaySelectedAnimations(int index)
        {
            PerformActionOnSelectedAnimations(index, animation => animation.PreviewAnimation());
        }

        public void StopSelectedAnimations(int index)
        {
            PerformActionOnSelectedAnimations(index, animation => animation.StopAnimation());
        }
    }
}