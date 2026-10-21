using System;
using UnityEngine;

namespace UtilSNR.Attributes
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
}
