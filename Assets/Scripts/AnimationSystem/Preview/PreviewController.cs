using System.Collections.Generic;
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

        public void PlaySelectedAnimations(int index)
        {
            if (index < 0 || index >= AnimationCollections.Count) return;

            CollectionPreview collection = AnimationCollections[index];

            foreach (AnimationType animationType in _playAnimationTypes.Keys)
            {
                if (_playAnimationTypes[animationType] == false) continue;

                switch (animationType)
                {
                    case AnimationType.Color:
                        if (collection.ColorAnimations != null)
                        {
                            foreach (ColorAnimation animation in collection.ColorAnimations)
                            {
                                animation?.PreviewAnimation();
                            }
                        }
                        break;
                    case AnimationType.Scale:
                        if (collection.ScaleAnimations != null)
                        {
                            foreach (ScaleAnimation animation in collection.ScaleAnimations)
                            {
                                animation?.PreviewAnimation();
                            }
                        }
                        break;
                    case AnimationType.Rotate:
                        if (collection.RotateAnimations != null)
                        {
                            foreach (RotateAnimation animation in collection.RotateAnimations)
                            {
                                animation?.PreviewAnimation();
                            }
                        }
                        break;
                    case AnimationType.Move:
                        if (collection.MoveAnimations != null)
                        {
                            foreach (MoveAnimation animation in collection.MoveAnimations)
                            {
                                animation?.PreviewAnimation();
                            }
                        }
                        break;
                    case AnimationType.Fade:
                        if (collection.FadeAnimations != null)
                        {
                            foreach (FadeAnimation animation in collection.FadeAnimations)
                            {
                                animation?.PreviewAnimation();
                            }
                        }
                        break;
                }
            }
        }

        public void StopSelectedAnimations(int index)
        {
            if (index < 0 || index >= AnimationCollections.Count) return;

            CollectionPreview collection = AnimationCollections[index];

            foreach (AnimationType animationType in _playAnimationTypes.Keys)
            {
                if (_playAnimationTypes[animationType] == false) continue;

                switch (animationType)
                {
                    case AnimationType.Color:
                        if (collection.ColorAnimations != null)
                        {
                            foreach (ColorAnimation animation in collection.ColorAnimations)
                            {
                                animation?.StopAnimation();
                            }
                        }
                        break;
                    case AnimationType.Scale:
                        if (collection.ScaleAnimations != null)
                        {
                            foreach (ScaleAnimation animation in collection.ScaleAnimations)
                            {
                                animation?.StopAnimation();
                            }
                        }
                        break;
                    case AnimationType.Rotate:
                        if (collection.RotateAnimations != null)
                        {
                            foreach (RotateAnimation animation in collection.RotateAnimations)
                            {
                                animation?.StopAnimation();
                            }
                        }
                        break;
                    case AnimationType.Move:
                        if (collection.MoveAnimations != null)
                        {
                            foreach (MoveAnimation animation in collection.MoveAnimations)
                            {
                                animation?.StopAnimation();
                            }
                        }
                        break;
                    case AnimationType.Fade:
                        if (collection.FadeAnimations != null)
                        {
                            foreach (FadeAnimation animation in collection.FadeAnimations)
                            {
                                animation?.StopAnimation();
                            }
                        }
                        break;
                }
            }
        }
    }
}