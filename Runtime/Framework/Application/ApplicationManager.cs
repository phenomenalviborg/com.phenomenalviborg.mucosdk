using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : PhenomenalViborg.MUCOSDK.IManager<ApplicationManager>
    {
        public static ApplicationConfiguration applicationConfiguration { get; private set; } = null;
        public static ApplicationManager applicationManager { get; private set; } = null;
        public static MUCOThreadManager threadManager { get; private set; } = null;
        public static TrackingManager trackingManager { get; private set; } = null;
        public static ClientNetworkManager clientNetworkManager { get; private set; } = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void EntryPoint()
        {
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
            managersGameObject.SetActive(false);
            applicationManager = managersGameObject.AddComponent<ApplicationManager>();
            threadManager = managersGameObject.AddComponent<MUCOThreadManager>();
            trackingManager = managersGameObject.AddComponent<TrackingManager>();
            clientNetworkManager = managersGameObject.AddComponent<ClientNetworkManager>();
            managersGameObject.SetActive(true);
            Debug.Log(applicationManager);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Log("Initializing ApplicationManager...");
        }

        private void Start()
        {
#if UNITY_EDITOR
            Debug.Log("Trying to load active scene as experience.");
            ExperienceConfiguration experienceConfiguration = applicationConfiguration.ExperienceConfigurations.Find(e => e.Scene.sceneIndex == EditorSceneManager.GetActiveScene().buildIndex);
            if (experienceConfiguration == null)
            {
                Debug.LogError("Failed to find active scene in the registered expereience configurations.");
                return;
            }

            applicationManager.LoadExperienceByConfiguration(experienceConfiguration);
#endif
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

            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(experienceConfiguration.Scene.sceneIndex);

            // Wait until scene loading has completed
            while (!loadSceneAsync.isDone)
            {
                yield return null;
            }

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