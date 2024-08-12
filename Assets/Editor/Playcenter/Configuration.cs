using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Playcenter
{
    public class Configuration : EditorWindow
    {
        private static readonly List<string> Defines = new List<string>
        {
            "EOS_MULTIPLAYER",
            "NAKAMA_MULTIPLAYER"
        };

        [MenuItem("Playcenter/Configuration")]
        private static void ShowWindow()
        {
            var window = GetWindow<Configuration>();
            window.titleContent = new GUIContent("Configuration");
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.LockReloadAssemblies();
        }
        
        private void OnDisable()
        {
            EditorApplication.UnlockReloadAssemblies();
        }

        private void OnGUI()
        {
            // Create a bool field for each android and ios for enable or disable the platform specific defines
            GUILayout.Label("Platform Specific Defines", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Android", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            foreach (var define in Defines)
            {
                EditorGUILayout.BeginHorizontal();
                bool currentDefineState = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android).Contains(define);
                bool newDefineState = EditorGUILayout.Toggle(define, currentDefineState);
                if (newDefineState != currentDefineState)
                {
                    ApplyDefineChange(define, newDefineState, NamedBuildTarget.Android);
                    ApplyDefineChange(define, newDefineState, NamedBuildTarget.Standalone);
                }
                EditorGUILayout.EndHorizontal();
;            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("iOS", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            foreach (var define in Defines)
            {
                EditorGUILayout.BeginHorizontal();
                bool currentDefineState = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.iOS).Contains(define);
                bool newDefineState = EditorGUILayout.Toggle(define, currentDefineState);
                if (newDefineState != currentDefineState)
                {
                    ApplyDefineChange(define, newDefineState, NamedBuildTarget.iOS);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyDefineChange(string define, bool enable, NamedBuildTarget targetPlatform)
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(targetPlatform);
            List<string> defineList = new List<string>(currentDefines.Split(';'));

            switch (enable)
            {
                case true when !defineList.Contains(define):
                    defineList.Add(define);
                    break;
                case false when defineList.Contains(define):
                    defineList.Remove(define);
                    break;
            }

            PlayerSettings.SetScriptingDefineSymbols(targetPlatform, string.Join(";", defineList.ToArray()));
        }
    }
}
