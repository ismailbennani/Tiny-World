using System;
using System.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Utils.Animation
{
    public class ShakeAnimation : MonoBehaviour, IAnimation
    {
        public ShakeAnimationParameters xAxisShake;
        public ShakeAnimationParameters yAxisShake;
        public ShakeAnimationParameters zAxisShake;
        public float duration;

        public IEnumerator Animate()
        {
            if (duration == 0)
            {
                duration = 0.1f;
            }
            
            float startTime = Time.time;
            float endTime = startTime + duration;

            Vector3 originalPosition = transform.position;

            while (Time.time <= endTime)
            {
                float time = (Time.time - startTime) / duration;

                float displacementX = xAxisShake.GetDisplacement(time);
                float displacementY = yAxisShake.GetDisplacement(time);
                float displacementZ = zAxisShake.GetDisplacement(time);

                Vector3 displacement = new(displacementX, displacementY, displacementZ);

                transform.position = originalPosition + displacement;

                yield return null;
            }
        }
    }

    [Serializable]
    public class ShakeAnimationParameters
    {
        public ShakeAnimationType shakeType;

        public AnimationCurve frequencyProfile = AnimationCurve.Constant(0, 1, 10);
        public AnimationCurve amplitudeProfile = AnimationCurve.Constant(0, 1, 0.1f);
        public float phase;

        public float GetDisplacement(float time)
        {
            switch (shakeType)
            {
                case ShakeAnimationType.None:
                    return 0;
                case ShakeAnimationType.Sin:
                    return amplitudeProfile.Evaluate(time) * Mathf.Sin(frequencyProfile.Evaluate(time) * time + phase);
                case ShakeAnimationType.Cos:
                    return amplitudeProfile.Evaluate(time) * Mathf.Cos(frequencyProfile.Evaluate(time) * time + phase);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shakeType), shakeType, null);
            }
        }
    }

    public enum ShakeAnimationType
    {
        None,
        Sin,
        Cos,
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(ShakeAnimation), true)]
    public class ShakeAnimationEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();

            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(ShakeAnimationParameters))]
    public class ShakeAnimationParametersPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            Foldout foldout = new() { text = property.displayName };
            container.Add(foldout);

            PropertyField typeField;
            PropertyField frequencyProfileField;
            PropertyField amplitudeProfileField;
            VisualElement phaseContainer;
            Box parametersContainer;
            {
                typeField = new PropertyField(property.FindPropertyRelative("shakeType"));
                foldout.Add(typeField);

                parametersContainer = new Box { viewDataKey = property.name + "-foldout", style = { marginLeft = new StyleLength(15) } };
                foldout.Add(parametersContainer);

                frequencyProfileField = new PropertyField(property.FindPropertyRelative("frequencyProfile"));
                parametersContainer.Add(frequencyProfileField);

                amplitudeProfileField = new PropertyField(property.FindPropertyRelative("amplitudeProfile"));
                parametersContainer.Add(amplitudeProfileField);

                phaseContainer = new VisualElement { style = { flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row) } };
                parametersContainer.Add(phaseContainer);

                PropertyField phaseField = new(property.FindPropertyRelative("phase")) { style = { flexGrow = 1 } };
                phaseContainer.Add(phaseField);

                Button randomizeButton = new()
                {
                    text = "R",
                    style = { marginLeft = new StyleLength(new Length(2, LengthUnit.Pixel)), marginRight = 0 }
                };
                randomizeButton.RegisterCallback<ClickEvent, SerializedProperty>(
                    (_, p) =>
                    {
                        p.floatValue = Random.Range(0, 2 * Mathf.PI);
                        p.serializedObject.ApplyModifiedProperties();
                    },
                    property.FindPropertyRelative("phase")
                );
                phaseContainer.Add(randomizeButton);
            }

            typeField.RegisterValueChangeCallback(_ =>
            {
                RecreateParameters(property, frequencyProfileField, amplitudeProfileField, phaseContainer);
                parametersContainer.MarkDirtyRepaint();
            });

            RecreateParameters(property, frequencyProfileField, amplitudeProfileField, phaseContainer);

            return container;
        }

        private void RecreateParameters(SerializedProperty property, params VisualElement[] elements)
        {
            ShakeAnimationType type = (ShakeAnimationType)property.FindPropertyRelative("shakeType").intValue;

            switch (type)
            {
                case ShakeAnimationType.Sin:
                case ShakeAnimationType.Cos:
                    foreach (VisualElement element in elements)
                    {
                        element.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    }
                    break;
                case ShakeAnimationType.None:
                default:
                    foreach (VisualElement element in elements)
                    {
                        element.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    }
                    break;
            }
        }
    }

    #endif
}
