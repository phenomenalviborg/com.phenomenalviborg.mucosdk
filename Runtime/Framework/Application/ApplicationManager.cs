using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : PhenomenalViborg.MUCOSDK.IManager<ApplicationManager>
    {
        public static ApplicationConfiguration applicationConfiguration { get; private set; } = null;
        public static ApplicationManager applicationManager { get; private set; } = null;
        public static MUCOThreadManager threadManager { get; private set; } = null;
        public static TrackingManager trackingManager { get; private set; } = null;
        public static ClientNetworkManager clientNetworkManager { get; private set; } = null;

        public static bool initialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EntryPoint()
        {
            if (initialized)
            {
                return;
            }
            else
            {
                initialized = true;
            }

            Debug.Log("Initializing MUCOSDK...");

            // Load application configuration
            applicationConfiguration = Resources.Load<ApplicationConfiguration>("ApplicationConfiguration");
            if (applicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
                return;
            }

            if (applicationConfiguration.ManualInitialization)
            {
                Debug.Log("Manual initialization is enabled, returning.");
                return;
            }

            if (applicationConfiguration.ExperienceConfigurations.Count < 1)
            {
                Debug.LogError("Experience configurations count was 0.");
                return;
            }

            // Initialize manager object
            GameObject managersGameObject = new GameObject("MUCOSDKManagers");
            DontDestroyOnLoad(managersGameObject);
            applicationManager = managersGameObject.AddComponent<ApplicationManager>();
            threadManager = managersGameObject.AddComponent<MUCOThreadManager>();
            clientNetworkManager = managersGameObject.AddComponent<ClientNetworkManager>();

            applicationManager.Initialized();
            threadManager.Initialized();
            clientNetworkManager.Initialized();

            Debug.Log("Entry point end");

            // Load experience
#if UNITY_EDITOR
            /*ExperienceConfiguration experienceConfiguration = applicationConfiguration.ExperienceConfigurations.Find(e => e.Scene.sceneIndex == EditorSceneManager.GetActiveScene().buildIndex);
            if (experienceConfiguration == null)
            {
                Debug.LogError("Failed to find active scene in the registered expereience configurations.");
                return;
            }*/
#else
#endif

            ExperienceConfiguration experienceConfiguration = applicationConfiguration.ExperienceConfigurations[0];
            if (experienceConfiguration == null)
            {
                Debug.LogError("Failed to find active scene in the registered expereience configurations.");
                return;
            }

            Debug.Log($"Trying to load active scene as experience. {experienceConfiguration.name}");
            applicationManager.LoadExperienceByConfiguration(experienceConfiguration);

        }

        public void Initialized()
        {
            Debug.Log("Initializing ApplicationManager...");
        }

        #region Experience loading
        public void LoadExperienceByName(string experienceName)
        {
            ExperienceConfiguration experienceConfiguration = applicationConfiguration.ExperienceConfigurations.Find(ec => ec.Name == experienceName);
            if (!experienceConfiguration)
            {
                Debug.LogError($"Failed to find experience configuration with name '{experienceName}' in the specified application configuration.");
                return;
            }

            LoadExperienceByConfiguration(experienceConfiguration);
        }

        public void LoadExperienceByConfiguration(ExperienceConfiguration experienceConfiguration)
        {
            bool found = applicationConfiguration.ExperienceConfigurations.Find(e => e == experienceConfiguration);
            if (!found)
            {
                Debug.LogError($"Failed to find experience configuration in the specified application configuration.");
                return;
            }

            StartCoroutine(LoadExperienceAsync(experienceConfiguration));
        }

        public IEnumerator LoadExperienceAsync(ExperienceConfiguration experienceConfiguration)
        {
            Debug.Log($"Loading experience '{experienceConfiguration.Name}'...");

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(experienceConfiguration.Scene.sceneIndex);
            asyncOperation.allowSceneActivation = true;

            // Wait until scene loading has completed
            while (!asyncOperation.isDone)
            {
                Debug.Log($"Loading scene... {asyncOperation.progress * 100}%");
                yield return null;
            }

            Debug.Log("Finished loading scene.");

            // Initialize ExperienceManager
            GameObject gameObject = new GameObject("MUCOExperience");
            ExperienceManager experienceManager = gameObject.AddComponent<ExperienceManager>();
            experienceManager.Initialize(experienceConfiguration);
            DontDestroyOnLoad(experienceManager);

            Debug.Log("Finished loading experience.");
        }
        #endregion
    }
}