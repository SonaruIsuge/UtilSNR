using System;
using UnityEditor;
using UnityEngine;

namespace UtilSNR.Editor.Utils
{
    public class EditorHelper : MonoBehaviour
    {
        public static GUILayoutOption[] GetSizeOption(float width, float height)
        {
            GUILayoutOption[] options = new GUILayoutOption[]{
                    GUILayout.MinWidth(width),
                    GUILayout.MinHeight(height),
                    GUILayout.MaxWidth(width),
                    GUILayout.MaxHeight(height),
        };
            return options;
        }

        public static GUILayoutOption[] GetSizeOption(Vector2 size)
        {
            GUILayoutOption[] options = new GUILayoutOption[]{
                    GUILayout.MinWidth(size.x),
                    GUILayout.MinHeight(size.y),
                    GUILayout.MaxWidth(size.x),
                    GUILayout.MaxHeight(size.y),
        };
            return options;
        }

        public static void Horizontal(Action block, GUIStyle style = null)
        {
            if (style == null)
                GUILayout.BeginHorizontal();
            else
                GUILayout.BeginHorizontal(style);

            block();
            GUILayout.EndHorizontal();
        }

        public static void Vertical(Action block, GUIStyle style = null)
        {
            if (style == null)
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(style);

            block();
            GUILayout.EndVertical();
        }

        public static void Button(string text, Action click = null)
        {
            if (GUILayout.Button(text))
            {
                click?.Invoke();
            }
        }

        public static void Button(Texture texture, Action click = null)
        {
            if (GUILayout.Button(texture))
            {
                click?.Invoke();
            }
        }

        public static void Button(string text, Vector2 size, Action click = null)
        {
            if (GUILayout.Button(text, GetSizeOption(size.x, size.y)))
            {
                click();
            }
        }

        public static void Button(Texture texture, Vector2 size, Action click = null)
        {
            if (GUILayout.Button(texture, GetSizeOption(size.x, size.y)))
            {
                click();
            }
        }

        public static void Label(string text, Vector2 size, GUIStyle labelStyle = null, int fontSize = 10, TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            if (labelStyle == null)
            {
                labelStyle = GUI.skin.label;
                labelStyle.alignment = textAnchor;
                labelStyle.fontSize = fontSize;
            }

            EditorGUILayout.LabelField(text, labelStyle, GetSizeOption(size.x, size.y));
        }

        public static void Label(Texture texture, Vector2 size, GUIStyle labelStyle = null)
        {
            if (labelStyle == null)
                GUILayout.Label(texture, GetSizeOption(size.x, size.y));
            else
                GUILayout.Label(texture, labelStyle, GetSizeOption(size.x, size.y));
        }

        public static void Label(string text, Vector2 size, TextAnchor textAnchor)
        {
            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.alignment = textAnchor;

            GUILayout.Label(text, labelStyle, GetSizeOption(size));
        }

        public static void Label(string text, Vector2 size, int fontSize)
        {
            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.fontSize = fontSize;

            GUILayout.Label(text, labelStyle, GetSizeOption(size));
        }


        public static void Label(string text, int fontSize = 10, TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.alignment = textAnchor;
            labelStyle.fontSize = fontSize;

            EditorGUILayout.LabelField(text, labelStyle);
        }

        public static void LayoutHorizontal(Action block, TextAlignment align = TextAlignment.Center, GUIStyle style = null)
        {
            Action layoutBlock;
            switch (align)
            {
                default:
                case TextAlignment.Center:
                    layoutBlock = new Action(() => {
                        GUILayout.FlexibleSpace();
                        block();
                        GUILayout.FlexibleSpace();
                    });
                    break;
                case TextAlignment.Left:
                    layoutBlock = new Action(() => {
                        block();
                        GUILayout.FlexibleSpace();
                    });
                    break;
                case TextAlignment.Right:
                    layoutBlock = new Action(() => {
                        GUILayout.FlexibleSpace();
                        block();
                    });
                    break;
            }
            Horizontal(layoutBlock, style);
        }

        public static void LayoutVerticalCenter(Action block, TextAlignment align = TextAlignment.Center, GUIStyle style = null)
        {
            Action layoutBlock;
            layoutBlock = new Action(() => {
                GUILayout.FlexibleSpace();
                LayoutHorizontal(() => block(), align, style);
                GUILayout.FlexibleSpace();
            });
            Vertical(layoutBlock, style);
        }

        public static void DrawLabeledElement(Action block, string labelName, float labelWidth = 100, GUIStyle style = null)
        {
            GUILayout.Space(5);
            Horizontal(() => {
                Label(labelName, new Vector2(labelWidth, 14), null, 11, TextAnchor.MiddleLeft);
                block();
            }, style);

        }

        public static void DrawTransparentTexture(Rect rect, Texture texture, Material material = null)
        {
            Color guiColor = GUI.color; // Save the current GUI color

            if (material == null)
            {
                GUI.color = Color.clear; // Set up transparent color
                EditorGUI.DrawTextureTransparent(rect, texture);
            }
            else
            {

            }

            GUI.color = guiColor; // Get back to previous GUI color
        }

        public static void AvarageSpaceObjects(Action[] blocks, int space)
        {
            GUILayout.Space(space);
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i]();
                GUILayout.Space(space);
            }
        }

        public static void AvarageSpaceObjects(Action[] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i]();

                if (i != blocks.Length - 1)
                    GUILayout.FlexibleSpace();
            }
        }

        public static void DropDown(string name, string[] options, ref int selectOption, Vector2 size)
        {
            GUIStyle style = GUI.skin.GetStyle("Popup");
            style.alignment = TextAnchor.MiddleCenter;
            selectOption = EditorGUILayout.Popup(
                    name, selectOption, options, style,
                    GetSizeOption(size));
        }

        public static void DropDown(string name, string[] options, ref int selectOption)
        {
            GUIStyle style = GUI.skin.GetStyle("Popup");
            style.alignment = TextAnchor.MiddleCenter;
            selectOption = EditorGUILayout.Popup(
                    name, selectOption, options, style);
        }
    }
}
