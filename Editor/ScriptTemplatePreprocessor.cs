//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Script Template Preprocessor
//             Author: Sonaru
//             Date Created: 12nd June, 2026
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//        
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

namespace UtilSNR.Editor
{
    /// <summary>
    /// This Preprocessor script runs on all new C# files created. It will
    /// allow us to automatically replace macros with text we want it to have.
    /// For example, automatically assigning the current data and nicifying the
    /// the script name for the title.
    /// </summary>
    public class ScriptTemplatePreprocessor : AssetModificationProcessor
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *            Declarations
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private const string DefaultAuthor = "SkeletonCrew";

        private static readonly Dictionary<string, Func<string, string>> MacrosToValues = new Dictionary<string, Func<string, string>>()
        {
            { "#SCRIPT_TITLE#",      (path) => GetScriptTitle(path) },
            { "#DATE#",              (path) => GetDate() },
            { "#PROJECT_NAME#",      (path) => GetProjectName() },
            { "#AUTHOR#",            (path) => GetAuthor() },
        };

        private static readonly string[] DaysInMonth = new string[31] 
        { 
            "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", 
            "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st",
            "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st" 
        };

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *            Unity Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static void OnWillCreateAsset(string path)
        {
            // Trigger on .meta creation because it ensures the .cs file is already on disk.
            if (path.EndsWith(".cs.meta") == false
                || Path.GetFileNameWithoutExtension(path) == nameof(ScriptTemplatePreprocessor))
            {
                return;
            }

            // Removing .meta from filepath
            string csFilePath = path.Substring(0, path.Length - 5);

            // Using delayCall to avoid modifying the file while Unity is in the middle of an AssetDatabase operation.
            EditorApplication.delayCall += () =>
            {
                ProcessScript(csFilePath);
            };
        }

        private static void ProcessScript(string csFilePath)
        {
            if (!File.Exists(csFilePath))
            {
                return;
            }

            try
            {
                string contents = File.ReadAllText(csFilePath);
                bool modified = false;

                foreach (var macro in MacrosToValues)
                {
                    if (contents.Contains(macro.Key))
                    {
                        contents = contents.Replace(macro.Key, macro.Value(csFilePath));
                        modified = true;
                    }
                }

                if (modified)
                {
                    File.WriteAllText(csFilePath, contents);
                    
                    // Trigger a targeted re-import now that we're outside the initial creation hook.
                    AssetDatabase.ImportAsset(csFilePath, ImportAssetOptions.ForceUpdate);
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"[ScriptTemplatePreprocessor] Failed to parse script template ({exception.GetType()}): {exception.Message}");
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // *            Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static string GetScriptTitle(string path)
        {
            string className = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(className)) return "Unknown";

            string title = $"{char.ToUpper(className[0])}";
            int classNameLength = className.Length;
            for (int i = 1; i < classNameLength; ++i)
            {
                if (char.IsUpper(className[i]))
                {
                    // Capital Letter => Space, then letter. Unless there are two capital letters in a row. In which case leave them next to one another.
                    // If a Space or Underscore is already found as the previous character, also skip adding a new space.
                    char previousChar = className[i - 1];
                    if (char.IsUpper(previousChar) == false && previousChar != ' ' && previousChar != '_')
                    {
                        title += $" {className[i]}";
                        continue;
                    }
                }
                else if (className[i] == '_')
                {
                    // Underscore => Replace with space
                    title += ' ';
                    continue;
                }

                title += className[i];
            }
            return title;
        }

        private static string GetDate()
        {
            DateTime now = DateTime.Now;
            return $"{DaysInMonth[now.Day - 1]} {now.ToString("MMMM, yyyy")}";
        }

        private static string GetProjectName()
        {
            if (string.IsNullOrEmpty(PlayerSettings.productName) == false)
            {
                return PlayerSettings.productName;
            }
            return $"#PROJECT_NAME# is undefined. Please insert a {nameof(PlayerSettings.productName)} in the {nameof(PlayerSettings)}.";
        }

        private static string GetAuthor()
        {
            string name = RunGitCommand("config user.name");
            string email = RunGitCommand("config user.email");

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                return $"{name} <{email}>";
            }

            if (!string.IsNullOrEmpty(name)) return name;

            return DefaultAuthor;
        }

        private static string RunGitCommand(string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        return result.Trim();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
#endif