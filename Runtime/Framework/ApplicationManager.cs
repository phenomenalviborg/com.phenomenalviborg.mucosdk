using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : PhenomenalViborg.MUCOSDK.IManager<ApplicationManager>
    {
        [SerializeField] private ApplicationConfiguration m_ApplicationConfiguration;

        public ApplicationConfiguration GetApplicationConfiguration() { return m_ApplicationConfiguration; }

        private void Awake()
        {
            base.Awake();

            m_ApplicationConfiguration = Resources.Load<ApplicationConfiguration>("ApplicationConfiguration");
            if (m_ApplicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
            }
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene(m_ApplicationConfiguration.MenuScene.sceneIndex);

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

            if (m_ApplicationConfiguration.OfflineMode)
            {
                NetworkUser networkUser;
                networkUser.Identifier = 0;
                networkUser.IsLocalUser = true;
                clientNetworkManager.AddNetworkUser(networkUser);

                if (m_ApplicationConfiguration.ExperienceConfigurations.Count() > 0)
                {
                    LoadExperienceByName(m_ApplicationConfiguration.ExperienceConfigurations[0].Name);                
                }
                else
                {
                    Debug.Log("Failed to start offline mode, experience configurations count was 0.");
                }
            }
            else
            {
                string serverAddress = trackingManager.GetStringPropertyFromAdminNode("ServerAddress");
                int serverPort = int.Parse(trackingManager.GetStringPropertyFromAdminNode("ServerPort"));
                clientNetworkManager.Connect(serverAddress, serverPort);
            }
        }

        public void LoadExperienceByName(string experienceName)
        {
            ExperienceConfiguration experienceConfiguration = m_ApplicationConfiguration.ExperienceConfigurations.Find(ec => ec.Name == experienceName);
            if (!experienceConfiguration)
            {
                Debug.LogError($"Failed to find experience configuration with name '{experienceName}' in the specified application configuration.");
            }

            StartCoroutine(LoadExperienceAsync(experienceConfiguration));
        }

        public IEnumerator LoadExperienceAsync(ExperienceConfiguration experienceConfiguration)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(experienceConfiguration.Scene.sceneIndex);

            // Wait until scene loading has completed
            while (!loadSceneAsync.isDone)
            {
                yield return null;
            }

            // TODO: Find ExperienceManager class from ExperienceConfiguration
            GameObject gameObject = new GameObject("MUCOExperience");
            ExperienceManager experienceManager = gameObject.AddComponent<ExperienceManager>();
            experienceManager.Initialize(experienceConfiguration);
        }
    }
}