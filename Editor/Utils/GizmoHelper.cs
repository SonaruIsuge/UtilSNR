using UnityEditor;
using UnityEngine;

namespace UtilSNR.Editor.Utils
{
    public static class GizmoHelper
    {
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
#if UNITY_EDITOR
            Color oldColor = Handles.color;
            Handles.color = color;

            Handles.DrawWireDisc(center, normal, radius);

            Handles.color = oldColor;
#endif
        }
    }
}
