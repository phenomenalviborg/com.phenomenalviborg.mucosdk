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
            string relativeApplicationConfigurationPath = $"Assets/ApplicationConfiguration.asset";
            m_ApplicationConfiguration = AssetDatabase.LoadAssetAtPath<ApplicationConfiguration>(relativeApplicationConfigurationPath);
            if (m_ApplicationConfiguration == null)
            {
                Debug.LogError("Failed to find application configuration!");
            }

            if (!m_ApplicationConfiguration.ManualInitialization)
            {
                return;
            }

            MUCOApplication.Initialize();
        }
    }

    public static class MUCOApplication
    {
        static ApplicationManager s_ApplicationManager = null;
        static MUCOThreadManager s_ThreadManager = null;
        static TrackingManager s_TrackingManager = null;
        static ClientNetworkManager s_ClientNetworkManager = null;

        static bool s_Initialized = false;
        public static void Initialize()
        {
            if (s_Initialized == true) return;

            Debug.Log("This runs now...");

            // WARN: GameObject creation HAS to happen on unity's main thread.
            GameObject gameObject = new GameObject("MUCOSDK");
            s_ApplicationManager = new ApplicationManager();
            // TODO: There are not reason for all these components to be MonoBehaviours, change it!
            s_ThreadManager = gameObject.AddComponent<MUCOThreadManager>();
            s_TrackingManager = gameObject.AddComponent<TrackingManager>();
            s_ClientNetworkManager = gameObject.AddComponent<ClientNetworkManager>();

            s_Initialized = true;
        }
    }
}
