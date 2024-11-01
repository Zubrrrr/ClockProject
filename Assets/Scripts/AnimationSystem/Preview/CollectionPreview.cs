using System.Collections.Generic;
using UnityEngine;

namespace AnimationSystem
{
    [System.Serializable]
    public class CollectionPreview
    {
        [Tooltip("The name of the collection")]
        public string CollectionName;

        public List<ColorAnimation> ColorAnimations = new List<ColorAnimation>();
        public List<ScaleAnimation> ScaleAnimations = new List<ScaleAnimation>();
        public List<RotateAnimation> RotateAnimations = new List<RotateAnimation>();
        public List<MoveAnimation> MoveAnimations = new List<MoveAnimation>();
        public List<FadeAnimation> FadeAnimations = new List<FadeAnimation>();
    }
}