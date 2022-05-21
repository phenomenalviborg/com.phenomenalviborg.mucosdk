using System.Linq;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class TrackingManager : PhenomenalViborg.MUCOSDK.IManager<TrackingManager>
    {
        [SerializeField] private string m_EnvironmentCode;

        private Antilatency.SDK.DeviceNetwork m_DeviceNetwork;
        private Antilatency.SDK.AltEnvironmentCode m_Environment;
        private Antilatency.SDK.AltEnvironmentMarkersDrawer m_AltEnvironmentMarkersDrawer;

        private Antilatency.Alt.Tracking.ILibrary m_TrackingLibrary;

        private Antilatency.DeviceNetwork.NodeHandle m_AdminNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;
        private Antilatency.DeviceNetwork.NodeHandle m_UserNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;

        public Antilatency.SDK.DeviceNetwork GetDeviceNetwork() { return m_DeviceNetwork; }
        public Antilatency.SDK.AltEnvironmentComponent GetEnvironment() { return m_Environment; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            m_DeviceNetwork = gameObject.AddComponent<Antilatency.SDK.DeviceNetwork>();
            
            m_Environment = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentCode>();
            m_Environment.EnvironmentCode = m_EnvironmentCode;

            // WARN: This causes a crash!
            // m_AltEnvironmentMarkersDrawer = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentMarkersDrawer>();
            // m_AltEnvironmentMarkersDrawer.Environment = m_Environment;

            m_TrackingLibrary = Antilatency.Alt.Tracking.Library.load();
            if (m_TrackingLibrary == null)
            {
                Debug.LogError("Failed to create tracking library");
                return;
            }
        }

        private void Start()
        {
            Antilatency.DeviceNetwork.NodeHandle[] compatibleAdminNodes = GetIdleTrackerNodesBySocketTag("Admin");
            m_AdminNodeHandle = compatibleAdminNodes.Length > 0 ? compatibleAdminNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogError("Failed to find tracking admin node.");
            }

            Antilatency.DeviceNetwork.NodeHandle[] compatibleUserNodes = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User");
            m_UserNodeHandle = compatibleUserNodes.Length > 0 ? compatibleUserNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogError("Failed to find tracking user node.");
            }
        }

        public string GetStringPropertyFromAdminNode(string key)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            return m_AdminNodeHandle != Antilatency.DeviceNetwork.NodeHandle.Null ? nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(m_AdminNodeHandle), key) : "";
        }

        public byte[] GetBinaryPropertyFromAdminNode(string key)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            return m_AdminNodeHandle != Antilatency.DeviceNetwork.NodeHandle.Null ? nativeNetwork.nodeGetBinaryProperty(nativeNetwork.nodeGetParent(m_AdminNodeHandle), key) : new byte[0];
        }

        private Antilatency.DeviceNetwork.NodeHandle[] GetUsbConnectedIdleIdleTrackerNodesBySocketTag(string socketTag)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            if (nativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetParent(nativeNetwork.nodeGetParent(v)) == Antilatency.DeviceNetwork.NodeHandle.Null &&
                        nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        private Antilatency.DeviceNetwork.NodeHandle[] GetIdleTrackerNodesBySocketTag(string socketTag)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            if (nativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }
    }
}
