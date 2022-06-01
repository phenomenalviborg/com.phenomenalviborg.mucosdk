using System.Linq;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class TrackingManager : PhenomenalViborg.MUCOSDK.IManager<TrackingManager>
    {
        private Antilatency.DeviceNetwork.ILibrary m_DeviceNetworkLibrary;
        private Antilatency.Alt.Tracking.ILibrary m_TrackingLibrary;
        private Antilatency.Alt.Environment.Selector.ILibrary m_EnvironmentSelectorLibrary;

        private Antilatency.DeviceNetwork.INetwork m_NativeNetwork;
        private Antilatency.Alt.Environment.IEnvironment m_Environment;

        private Antilatency.DeviceNetwork.NodeHandle m_AdminNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;
        private Antilatency.DeviceNetwork.NodeHandle m_UserNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;

        private void Start()
        {
            // Load device network library.
            m_DeviceNetworkLibrary = Antilatency.DeviceNetwork.Library.load();
            if (m_DeviceNetworkLibrary == null)
            {
                Debug.LogError("Failed to load Antilatency device network library.");
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            var jni = _library.QueryInterface<AndroidJniWrapper.IAndroidJni>();
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity")) {
                    jni.initJni(IntPtr.Zero, activity.GetRawObject());
                }
            }
            jni.Dispose();
#endif

            m_DeviceNetworkLibrary.setLogLevel(Antilatency.DeviceNetwork.LogLevel.Info);

            // Load tracking library.
            m_TrackingLibrary = Antilatency.Alt.Tracking.Library.load();
            if (m_TrackingLibrary == null)
            {
                Debug.LogError("Failed to load Antilatency tracking library.");
                return;
            }

            // Load selector library.
            m_EnvironmentSelectorLibrary = Antilatency.Alt.Environment.Selector.Library.load();
            if (m_EnvironmentSelectorLibrary == null)
            {
                Debug.LogError("Failed to load Antilatency environment selector library.");
                return;
            }

            // Create device network.
            Antilatency.DeviceNetwork.IDeviceFilter deviceFilter = m_DeviceNetworkLibrary.createFilter();
            deviceFilter.addUsbDevice(new Antilatency.DeviceNetwork.UsbDeviceFilter { vid = Antilatency.DeviceNetwork.UsbVendorId.Antilatency, pid = 0x0000 });
            
            m_NativeNetwork = m_DeviceNetworkLibrary.createNetwork(deviceFilter);
            if (m_NativeNetwork == null)
            {
                Debug.LogError("Failed to create Antilatency device network.");
            }

            // Create environment from the code.
            m_Environment = m_EnvironmentSelectorLibrary.createEnvironment("AAVSaWdpZDEBBnllbGxvdwQDuFOJP9xGoD6vjmO9mpmZPgAAAAAAAAAAAJqZGT8DAgICAQEAAgACJFk");
            if (m_Environment == null)
            {
                Debug.LogError("Failed to create Antilatency environment");
                return;
            }

            // Get admin node
            Antilatency.DeviceNetwork.NodeHandle[] compatibleAdminNodes = GetIdleTrackerNodesBySocketTag("Admin");
            m_AdminNodeHandle = compatibleAdminNodes.Length > 0 ? compatibleAdminNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogWarning("Admin node handle was null.");
            }

            // Get user node
            Antilatency.DeviceNetwork.NodeHandle[] compatibleUserNodes = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User");
            m_UserNodeHandle = compatibleUserNodes.Length > 0 ? compatibleUserNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_UserNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogWarning("User node handle was null.");
            }
        }

        private void Update()
        {
            // Update device network.
            /*if (m_DeviceNetwork)
            {
                uint updateId = m_NativeNetwork.getUpdateId();
                if (updateId != m_LastNetworkUpdateID)
                {
                    DeviceNetworkChanged.Invoke();
                    m_LastNetworkUpdateID = updateId;
                }
            }*/
        }

        private void OnDestroy()
        {
            // Terminate device network
            if (m_NativeNetwork != null)
            {
                m_NativeNetwork.Dispose();
                m_NativeNetwork = null;
            }

            if (m_DeviceNetworkLibrary != null)
            {
                m_DeviceNetworkLibrary.Dispose();
                m_DeviceNetworkLibrary = null;
            }
        }

        public string GetStringPropertyFromAdminNode(string key)
        {
            if (m_NativeNetwork == null)
            {
                return null;
            }

            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                return null;
            }

            return m_NativeNetwork.nodeGetStringProperty(m_NativeNetwork.nodeGetParent(m_AdminNodeHandle), key);
        }

        public byte[] GetBinaryPropertyFromAdminNode(string key)
        {
            if (m_NativeNetwork == null)
            {
                return new byte[0];
            }

            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                return new byte[0];
            }

            return m_NativeNetwork.nodeGetBinaryProperty(m_NativeNetwork.nodeGetParent(m_AdminNodeHandle), key);
        }

        private Antilatency.DeviceNetwork.NodeHandle[] GetUsbConnectedIdleIdleTrackerNodesBySocketTag(string socketTag)
        {
            if (m_TrackingLibrary == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            if (m_NativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(m_NativeNetwork).Where(v =>
                        m_NativeNetwork.nodeGetParent(m_NativeNetwork.nodeGetParent(v)) == Antilatency.DeviceNetwork.NodeHandle.Null &&
                        m_NativeNetwork.nodeGetStringProperty(m_NativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        m_NativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        private Antilatency.DeviceNetwork.NodeHandle[] GetIdleTrackerNodesBySocketTag(string socketTag)
        {
            if (m_TrackingLibrary == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            if (m_NativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(m_NativeNetwork).Where(v =>
                        m_NativeNetwork.nodeGetStringProperty(m_NativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        m_NativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }
    }
}
