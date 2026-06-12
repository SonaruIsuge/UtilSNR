using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UtilSNR.Editor.Utils
{
    /// <summary>
    /// Custom attribute to conditionally show fields in the Unity Inspector based on the value of another field.
    /// Can be used for bools, enums, or any field with a specific value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionField { get; }
        public object CompareValue { get; }

        // ShowIf based on a bool field
        public ShowIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
            CompareValue = true;
        }

        // ShowIf based on a specific value (enum, int, etc.)
        public ShowIfAttribute(string conditionField, object compareValue)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Custom property drawer for the ShowIfAttribute. 
    /// It checks the specified condition field and compares its value to determine whether to 
    /// display the property in the Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsConditionMet(property, (ShowIfAttribute)attribute))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsConditionMet(property, (ShowIfAttribute)attribute))
                return 0f; // collapse the field completely

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool IsConditionMet(SerializedProperty property, ShowIfAttribute showIf)
        {
            // Get the parent object
            object target = property.serializedObject.targetObject;
            FieldInfo field = target.GetType().GetField(
                showIf.ConditionField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (field == null) return true;

            object value = field.GetValue(target);
            return value != null && value.Equals(showIf.CompareValue);
        }
    }

#endif

}