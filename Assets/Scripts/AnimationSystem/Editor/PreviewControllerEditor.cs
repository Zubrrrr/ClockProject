using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationSystem
{
    [CustomEditor(typeof(PreviewController))]
    public class PreviewControllerEditor : Editor
    {
        private bool[] _showAnimationsFoldout;

        private SerializedProperty animationCollectionsProp;

        private void OnEnable()
        {
            animationCollectionsProp = serializedObject.FindProperty("AnimationCollections");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PreviewController controller = (PreviewController)target;

            if (_showAnimationsFoldout == null || _showAnimationsFoldout.Length != controller.AnimationCollections.Count)
            {
                _showAnimationsFoldout = new bool[controller.AnimationCollections.Count];
            }

            EditorGUILayout.PropertyField(animationCollectionsProp, true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select Animation Types to Play", EditorStyles.boldLabel);

            for (int i = 0; i < controller.AnimationCollections.Count; i++)
            {
                CollectionPreview collection = controller.AnimationCollections[i];

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(collection.CollectionName, EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Play Selected Animations"))
                {
                    controller.PlaySelectedAnimations(i);
                }
                if (GUILayout.Button("Stop Selected Animations"))
                {
                    controller.StopSelectedAnimations(i);
                }

                EditorGUILayout.EndHorizontal();

                _showAnimationsFoldout[i] = EditorGUILayout.Foldout(_showAnimationsFoldout[i], "Show Animations");

                if (_showAnimationsFoldout[i])
                {
                    EditorGUI.indentLevel++;
                    DisplayAnimationsList(collection.ColorAnimations, "Color Animations");
                    DisplayAnimationsList(collection.ScaleAnimations, "Scale Animations");
                    DisplayAnimationsList(collection.RotateAnimations, "Rotate Animations");
                    DisplayAnimationsList(collection.MoveAnimations, "Move Animations");
                    DisplayAnimationsList(collection.FadeAnimations, "Fade Animations");
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayAnimationsList<T>(List<T> animations, string label) where T : BaseAnimation
        {
            if (animations != null && animations.Count > 0)
            {
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

                foreach (T animation in animations)
                {
                    if (animation != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(animation.name, animation, typeof(T), true);

                        if (GUILayout.Button("Play"))
                        {
                            animation.PreviewAnimation();
                        }
                        if (GUILayout.Button("Stop"))
                        {
                            animation.StopAnimation();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}