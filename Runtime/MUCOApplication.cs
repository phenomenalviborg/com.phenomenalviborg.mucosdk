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

        public static ApplicationManager GetApplicationManager() { return s_ApplicationManager; }

        static bool s_Initialized = false;
        public static void Initialize()
        {
            if (s_Initialized == true) return;

            Debug.Log("This runs now...");

            // WARN: GameObject creation HAS to happen on unity's main thread.
            GameObject managersGameObject = new GameObject("MUCOSDKManagers");
            s_ApplicationManager = new ApplicationManager();
            s_ThreadManager = managersGameObject.AddComponent<MUCOThreadManager>();
            s_TrackingManager = managersGameObject.AddComponent<TrackingManager>();
            s_ClientNetworkManager = managersGameObject.AddComponent<ClientNetworkManager>();

            s_Initialized = true;
        }
    }
}
