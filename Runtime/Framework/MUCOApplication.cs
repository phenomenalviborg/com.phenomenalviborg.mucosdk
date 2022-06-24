using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{    
    public class UnityInitializer
    {
        private static ApplicationConfiguration m_ApplicationConfiguration;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // TODO Unify with ApplicationManager
            m_ApplicationConfiguration = Resources.Load<ApplicationConfiguration>("ApplicationConfiguration");
            if (m_ApplicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
                return;
            }

            if (m_ApplicationConfiguration.ManualInitialization)
            {
                return;
            }

            MUCOApplication.Initialize();
        }
    }

    public static class MUCOApplication
    {
        private static ApplicationManager s_ApplicationManager = null;
        private static MUCOThreadManager s_ThreadManager = null;
        private static TrackingManager s_TrackingManager = null;
        private static ClientNetworkManager s_ClientNetworkManager = null;

        static ApplicationConfiguration s_ApplicationConfiguration = null;
        
        static bool s_Initialized = false;

        public static ApplicationManager GetApplicationManager() { return s_ApplicationManager; }

        public static ApplicationConfiguration GetApplicationConfiguration() { return s_ApplicationConfiguration; }


        public static void Initialize()
        {
            if (s_Initialized == true) return;

            s_ApplicationConfiguration = Resources.Load<ApplicationConfiguration>("ApplicationConfiguration");
            if (s_ApplicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
                return;
            }
            if (s_ApplicationConfiguration.ExperienceConfigurations.Count < 1)
            {
                Debug.LogError("Experience configurations count was 0.");
                return;
            }

            // WARN: GameObject creation HAS to happen on unity's main thread.
            GameObject managersGameObject = new GameObject("MUCOSDKManagers");
            Object.DontDestroyOnLoad(managersGameObject);
            s_ApplicationManager = managersGameObject.AddComponent<ApplicationManager>();
            s_ThreadManager = managersGameObject.AddComponent<MUCOThreadManager>();
            //s_TrackingManager = managersGameObject.AddComponent<TrackingManager>();
            s_ClientNetworkManager = managersGameObject.AddComponent<ClientNetworkManager>();

            // Spawn experience manager
            GameObject experienceManagerGameObject = new GameObject("MUCOSDKExperienceManager");
            Object.DontDestroyOnLoad(experienceManagerGameObject);
            ExperienceManager experienceManager = experienceManagerGameObject.AddComponent<ExperienceManager>();
            experienceManager.Initialize(s_ApplicationConfiguration.ExperienceConfigurations[0]);

            // Connect to server
            string serverAddress = s_ApplicationConfiguration.ServerAddress;
            int serverPort = s_ApplicationConfiguration.ServerPort;
            s_ClientNetworkManager.Connect(serverAddress, serverPort);
            
            s_Initialized = true;
        }
    }
}
