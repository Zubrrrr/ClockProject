using UnityEditor;
using UnityEngine;

namespace AnimationSystem
{
    [CustomEditor(typeof(BaseAnimation), true)]
    public class BaseAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseAnimation baseAnimation = (BaseAnimation)target;

            if (GUILayout.Button("Preview Animation"))
            {
                baseAnimation.PreviewAnimation();
            }
        }
    }
}