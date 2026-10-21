using System.Reflection;
using UnityEngine;
using UnityEditor;
using UtilSNR.Attributes;

namespace UtilSNR.Editor.Utils
{
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
}
