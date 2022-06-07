using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CustomEditor(typeof(ApplicationConfiguration))]
    public class ApplicationConfigurationEditor : Editor
    {
        private ApplicationConfiguration m_ApplicationConfiguration;


        private SerializedProperty m_EntrySceneProperty;
        private SerializedProperty m_MenuSceneProperty;
        private SerializedProperty m_ExperienceConfigurationsProperty;
        private SerializedProperty m_ManualInitializationProperty;
        private SerializedProperty m_OfflineModeProperty;

        void OnEnable()
        {
            m_ApplicationConfiguration = (ApplicationConfiguration)target;

            m_EntrySceneProperty = serializedObject.FindProperty("EntryScene");
            m_MenuSceneProperty = serializedObject.FindProperty("MenuScene");
            m_ExperienceConfigurationsProperty = serializedObject.FindProperty("ExperienceConfigurations");
            m_ManualInitializationProperty = serializedObject.FindProperty("ManualInitialization");
            m_OfflineModeProperty = serializedObject.FindProperty("OfflineMode");
        }

        void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        public override void OnInspectorGUI()
        {
            // TODO: Make MUCOEditorGUI libarary for all this repeated code.
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 18;
            headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField(m_ApplicationConfiguration.name, headerStyle);
            EditorGUILayout.Space(32);

            List<Tuple<string, MessageType>> errorMessages = new List<Tuple<string, MessageType>>();

            GuiLine();
            EditorGUILayout.Space(16);

            // TODO: Base class to handle error system   
            EditorGUILayout.PropertyField(m_EntrySceneProperty, new GUIContent("Entry Scene"));
            //if (m_ApplicationConfiguration.EntryScene.sceneIndex == -1) { errorMessages.Add(new Tuple<string, MessageType>($"Missing scene! Please specify an entry scene for the experience.", MessageType.Error)); }
            //else if (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_ApplicationConfiguration.EntryScene)) < 0) { errorMessages.Add(new Tuple<string, MessageType>($"Failed to find specified entry scene in build settings! Please verify that your scene path is included in the 'File->Build Setting->Scenes In Build' list.", MessageType.Error)); }

            EditorGUILayout.PropertyField(m_MenuSceneProperty, new GUIContent("Menu Scene"));
            //if (!m_ApplicationConfiguration.MenuScene) { errorMessages.Add(new Tuple<string, MessageType>($"Missing scene! Please specify a menu scene for the experience.", MessageType.Error)); }
            //else if (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_ApplicationConfiguration.MenuScene)) < 0) { errorMessages.Add(new Tuple<string, MessageType>($"Failed to find specified menu scene in build settings! Please verify that your scene path is included in the 'File->Build Setting->Scenes In Build' list.", MessageType.Error)); }

            EditorGUILayout.Space(16);
            EditorGUILayout.PropertyField(m_ExperienceConfigurationsProperty, new GUIContent("Experience Configurations"));

            EditorGUILayout.Space(16);
            EditorGUILayout.PropertyField(m_ManualInitializationProperty, new GUIContent("Manual Initialization"));

            EditorGUILayout.Space(16);
            EditorGUILayout.PropertyField(m_OfflineModeProperty, new GUIContent("Offline Mode"));

            // Debug messages
            if (errorMessages.Count > 0)
            {
                EditorGUILayout.Space(16);
                foreach (Tuple<string, MessageType> errorMessage in errorMessages)
                {
                    EditorGUILayout.HelpBox(errorMessage.Item1, errorMessage.Item2);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
