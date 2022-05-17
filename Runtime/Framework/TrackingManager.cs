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

        private Antilatency.DeviceNetwork.NodeHandle? m_AdminNodeHandle = null;
        private Antilatency.DeviceNetwork.NodeHandle? m_UserNodeHandle = null;

        public Antilatency.SDK.DeviceNetwork GetDeviceNetwork() { return m_DeviceNetwork; }
        public Antilatency.SDK.AltEnvironmentComponent GetEnvironment() { return m_Environment; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            m_DeviceNetwork = gameObject.AddComponent<Antilatency.SDK.DeviceNetwork>();
            
            m_Environment = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentCode>();
            m_Environment.EnvironmentCode = m_EnvironmentCode;

            m_AltEnvironmentMarkersDrawer = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentMarkersDrawer>();
            m_AltEnvironmentMarkersDrawer.Environment = m_Environment;

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
            m_AdminNodeHandle = compatibleAdminNodes.Length > 0 ? (Antilatency.DeviceNetwork.NodeHandle?)compatibleAdminNodes[0] : null;

            Antilatency.DeviceNetwork.NodeHandle[] compatibleUserNodes = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User");
            m_UserNodeHandle = compatibleUserNodes.Length > 0 ? (Antilatency.DeviceNetwork.NodeHandle?)compatibleUserNodes[0] : null;

            Debug.Log($"AdminNodeHandle: {m_AdminNodeHandle}");
            Debug.Log($"UserNodeHandle: {m_UserNodeHandle}");
        }

        public string GetStringPropertyFromAdminNode(string key)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            return m_AdminNodeHandle != null ? nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent((Antilatency.DeviceNetwork.NodeHandle)m_AdminNodeHandle), key) : "";
        }

        public byte[] GetBinaryPropertyFromAdminNode(string key)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            return m_AdminNodeHandle != null ? nativeNetwork.nodeGetBinaryProperty(nativeNetwork.nodeGetParent((Antilatency.DeviceNetwork.NodeHandle)m_AdminNodeHandle), key) : new byte[0];
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
