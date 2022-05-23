using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager
    {

        [SerializeField] private ApplicationConfiguration m_ApplicationConfiguration;
        public ApplicationManager()
        {
            m_ApplicationConfiguration = Resources.Load<ApplicationConfiguration>("ApplicationConfiguration");
            if (m_ApplicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
            }
        }
        public void LoadMenu()
        {
            SceneManager.LoadScene(m_ApplicationConfiguration.MenuScene.name);

            TrackingManager trackingManager = TrackingManager.GetInstance();
            if (trackingManager == null)
            {
                Debug.LogError("Failed to get TrackingManager");
                return;
            }

            ClientNetworkManager clientNetworkManager = ClientNetworkManager.GetInstance();
            if (clientNetworkManager == null)
            {
                Debug.LogError("Failed to get ClientNetworkManager");
                return;
            }

            string serverAddress = trackingManager.GetStringPropertyFromAdminNode("ServerAddress");
            int serverPort = int.Parse(trackingManager.GetStringPropertyFromAdminNode("ServerPort"));
            clientNetworkManager.Connect(serverAddress, serverPort);
        }
        public void LoadExperienceByName(string experienceName)
        {
            ExperienceConfiguration experienceConfiguration = m_ApplicationConfiguration.ExperienceConfigurations.Find(ec => ec.Name == experienceName);
            if (!experienceConfiguration)
            {
                Debug.LogError($"Failed to find experience configuration with name '{experienceName}' in the specified application configuration.");
            }

            Debug.Log($"Loading experience '{experienceConfiguration.Scene.name}'.");
            LoadExperienceAsync(experienceConfiguration);
        }
        public IEnumerator LoadExperienceAsync(ExperienceConfiguration experienceConfiguration)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(experienceConfiguration.Scene.name);

            // Wait until scene loading has completed
            while (!loadSceneAsync.isDone)
            {
                yield return null;
            }

            // TODO: Find ExperienceManager class from ExperienceConfiguration
            GameObject gameObject = new GameObject("MUCOExperience");
            ExperienceManager experienceManager = gameObject.AddComponent<ExperienceManager>();
            experienceManager.Initialize(experienceConfiguration);
            Debug.Log(experienceManager);
        }
    }
}