using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOProjectSettingsWindow : EditorWindow
    {
        [MenuItem("MUCOSDK/MUCO Project Settings")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MUCOProjectSettingsWindow));
        }
        void GuiLine(int i_height = 1)
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        enum EExperienceTemplate
        {
            Blank,
            Gallery
        }

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        string experienceName = "NewExperience";
        int m_SelectedExperienceTemplateIndex = 0;

        ApplicationConfiguration m_ApplicationConfiguration;

        private void OnGUI()
        {   
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 18;
            headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField("MUCO Project Settings", headerStyle);
            EditorGUILayout.Space(32);

            string relativeApplicationConfigurationPath = $"Assets/ApplicationConfiguration.asset";
            m_ApplicationConfiguration = AssetDatabase.LoadAssetAtPath<ApplicationConfiguration>(relativeApplicationConfigurationPath);
            
            // Setup project settings and other setup
            GuiLine();
            EditorGUILayout.Space(16);
            GUIStyle setupButtonStyle = new GUIStyle(GUI.skin.button);
            setupButtonStyle.richText = true;
            GUI.enabled = !m_ApplicationConfiguration;
            if (GUILayout.Button("<b>Magic setup button (<i>pssst... artist, click me!</i>)</b>", setupButtonStyle, GUILayout.Height(100)))
            {
                // Create application configuration
                // TODO: Get scenes in some from some sort of constant MUCOSDK config file.
                m_ApplicationConfiguration = ScriptableObject.CreateInstance<ApplicationConfiguration>();
                
                m_ApplicationConfiguration.EntryScene = new SceneReference(AssetDatabase.GUIDFromAssetPath("Packages/com.phenomenalviborg.mucosdk/Runtime/Framework/Shared/S_Entry.unity").ToString()); // TODO: Better solution for path
                m_ApplicationConfiguration.MenuScene = new SceneReference(AssetDatabase.GUIDFromAssetPath("Packages/com.phenomenalviborg.mucosdk/Runtime/Framework/Shared/S_Menu.unity").ToString()); // TODO: Better solution for path
                AssetDatabase.CreateAsset(m_ApplicationConfiguration, relativeApplicationConfigurationPath);

                // Add scenes to build settings
                MUCOEditorUtilities.AddSceneToBuild(m_ApplicationConfiguration.EntryScene);
                MUCOEditorUtilities.AddSceneToBuild(m_ApplicationConfiguration.MenuScene);
            }
            EditorGUILayout.Space(8);
            GUI.enabled = false;
            m_ApplicationConfiguration = EditorGUILayout.ObjectField("Application Configuration", m_ApplicationConfiguration, typeof(ApplicationConfiguration), false) as ApplicationConfiguration;
            GUI.enabled = true;

            EditorGUILayout.Space(16);

            // Project generator
            GuiLine();
            EditorGUILayout.Space(16);

            experienceName = EditorGUILayout.TextField("Experience Name", experienceName);

            List<ExperienceTemplateConfiguration> experienceTemplateConfigurations = FindAssetsByType<ExperienceTemplateConfiguration>();
            string[] experienceTemplateNames = experienceTemplateConfigurations.Select(x => x.Name).ToArray();
            m_SelectedExperienceTemplateIndex = EditorGUILayout.Popup("Project Template", m_SelectedExperienceTemplateIndex, experienceTemplateNames);
         
            if (GUILayout.Button("Generate Project"))
            {
                ExperienceTemplateConfiguration experienceTemplateConfiguration = experienceTemplateConfigurations[m_SelectedExperienceTemplateIndex];

                string relativeProjectPath = $"Assets/{experienceName}";
                string absoluteProjectPath = $"{Application.dataPath}/{experienceName}";
                Debug.Log($"Generating project at: '{absoluteProjectPath}'");

                if (!Directory.Exists(absoluteProjectPath))
                {
                    // Create directory
                    Directory.CreateDirectory(absoluteProjectPath);
                    AssetDatabase.Refresh();

                    // Create experiece scene, this should most likely be duplicating a template scene?
                    string relativeExperienceScenePath = $"{relativeProjectPath}/S_{experienceName}.unity";
                    AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(experienceTemplateConfiguration.Scene.guid), relativeExperienceScenePath);
                    SceneReference experienceScene = new SceneReference(AssetDatabase.GUIDFromAssetPath(relativeExperienceScenePath).ToString());
                    MUCOEditorUtilities.AddSceneToBuild(experienceScene);

                    // Create experience configuration
                    string relativeExperienceConfigurationPath = $"{relativeProjectPath}/{experienceName}Configuration.asset";
                    ExperienceConfiguration experienceConfiguration = ScriptableObject.CreateInstance<ExperienceConfiguration>();
                    experienceConfiguration.Name = experienceName;
                    experienceConfiguration.Scene = experienceScene;
                    experienceConfiguration.LocalUserPrefab = experienceTemplateConfiguration.LocalUserPrefab;
                    experienceConfiguration.RemoteUserPrefab = experienceTemplateConfiguration.RemoteUserPrefab;
                    AssetDatabase.CreateAsset(experienceConfiguration, relativeExperienceConfigurationPath);

                    // Append experience to application configuration
                    if (m_ApplicationConfiguration)
                    {
                        m_ApplicationConfiguration.ExperienceConfigurations.Append(experienceConfiguration);
                    }
                    else
                    {
                        Debug.LogError("m_ApplicationConfiguration was undefined!");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to create project, a directory at '{absoluteProjectPath}' already exists.");
                    return;
                }
            }
        }   
    }
}
