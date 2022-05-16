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

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene(m_ApplicationConfiguration.MenuScene.name);
            
            string serverAddress = TrackingManager.GetInstance().GetStringPropertyFromAdminNode("ServerAddress");
            int serverPort = int.Parse(TrackingManager.GetInstance().GetStringPropertyFromAdminNode("ServerPort"));
            ClientNetworkManager.GetInstance().Connect(serverAddress, serverPort);
        }

        public void LoadExperienceByName(string experienceName)
        {
            ExperienceConfiguration experienceConfiguration = m_ApplicationConfiguration.ExperienceConfigurations.Find(ec => ec.Name == experienceName);
            if (!experienceConfiguration)
            {
                Debug.LogError($"Failed to find experience configuration with name '{experienceName}' in the specified application configuration.");
            }

            Debug.Log($"Loading experience '{experienceConfiguration.Scene.name}'.");
            StartCoroutine(LoadExperienceAsync(experienceConfiguration));
        }

        public IEnumerator LoadExperienceAsync(ExperienceConfiguration experienceConfiguration)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(experienceConfiguration.Scene.name);

            // Wait until scene loading has completed
            while (!loadSceneAsync.isDone)
            {
                // TODO: Find ExperienceManager class from ExperienceConfiguration
                ExperienceManager experienceManager = gameObject.AddComponent<ExperienceManager>();
                experienceManager.Initialize(experienceConfiguration);

                Debug.Log(experienceManager);

                yield return null;
            }
        }
    }
}