using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CustomEditor(typeof(ExperienceConfiguration))]
    public class ExperienceConfigurationEditor : Editor
    {
        private ExperienceConfiguration m_ExperienceConfiguration;

        void OnEnable()
        {
            m_ExperienceConfiguration = (ExperienceConfiguration)target;
        }

        void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 18;
            headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField(m_ExperienceConfiguration.name, headerStyle);
            EditorGUILayout.Space(32);

            // Variables
            List<Tuple<string, MessageType>> errorMessages = new List<Tuple<string, MessageType>>();

            GuiLine();
            EditorGUILayout.Space(16);
            m_ExperienceConfiguration.Name = EditorGUILayout.TextField("Name", m_ExperienceConfiguration.Name);
            m_ExperienceConfiguration.Description = EditorGUILayout.TextField("Description", m_ExperienceConfiguration.Description);

            m_ExperienceConfiguration.Scene = EditorGUILayout.ObjectField("Scene", m_ExperienceConfiguration.Scene, typeof(SceneAsset), false) as SceneAsset;
            if (!m_ExperienceConfiguration.Scene) { errorMessages.Add(new Tuple<string, MessageType>($"Missing scene! Please specify a scene for the experience.", MessageType.Error)); }
            else if (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_ExperienceConfiguration.Scene)) < 0) { errorMessages.Add(new Tuple<string, MessageType>($"Failed to find specified scene in build settings! Please verify that your scene path is included in the 'File->Build Setting->Scenes In Build' list.", MessageType.Error)); }

            m_ExperienceConfiguration.LocalUserPrefab = EditorGUILayout.ObjectField("Local User Prefab", m_ExperienceConfiguration.LocalUserPrefab, typeof(GameObject), false) as GameObject;
            if (m_ExperienceConfiguration.LocalUserPrefab && !m_ExperienceConfiguration.LocalUserPrefab.GetComponent<User>()) { errorMessages.Add(new Tuple<string, MessageType>("Failed to find a 'User' component on 'LocalUserPrefab'! Please verify that your user prefabs has a 'User' component attached prefab root entity.", MessageType.Error)); }
           
            m_ExperienceConfiguration.RemoteUserPrefab = EditorGUILayout.ObjectField("Remote User Prefab", m_ExperienceConfiguration.RemoteUserPrefab, typeof(GameObject), false) as GameObject;
            if (m_ExperienceConfiguration.RemoteUserPrefab && !m_ExperienceConfiguration.RemoteUserPrefab.GetComponent<User>()) { errorMessages.Add(new Tuple<string, MessageType>("Failed to find a 'User' component on 'RemoteUserPrefab'! Please verify that your user prefabs has a 'User' component attached prefab root entity.", MessageType.Error)); }
          
            // Debug messages
            if (errorMessages.Count > 0)
            {
                EditorGUILayout.Space(16);
                foreach (Tuple<string, MessageType> errorMessage in errorMessages)
                {
                    EditorGUILayout.HelpBox(errorMessage.Item1, errorMessage.Item2);
                }
            }

            Repaint(); // Fix for bug where 'scene in build' error won't disappear until asset is refocused.
        }
    }
}
