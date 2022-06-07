    using System;
using System.Linq;
using UnityEngine.Events;
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

        private Antilatency.DeviceNetwork.NodeHandle m_UserNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;
        private Antilatency.DeviceNetwork.NodeHandle m_AdminNodeHandle = Antilatency.DeviceNetwork.NodeHandle.Null;

        private UnityEvent m_DeviceNetworkChanged = new UnityEvent();
        private uint m_LastUpdateId = 0;

        private Antilatency.Alt.Tracking.ITrackingCotask m_UserTrackingCotask;
        private Vector3 m_UserNodePosition = Vector3.zero;
        private Quaternion m_UserNodeRotation = Quaternion.identity;
        private float m_UserTrackingExtrapolationTime = 0.0f;
        private UnityEngine.Pose m_TrackingPlacement;

        private void Start()
        {
            // Load device network library.
            m_DeviceNetworkLibrary = Antilatency.DeviceNetwork.Library.load();
            if (m_DeviceNetworkLibrary == null)
            {
                Debug.LogError("Failed to load Antilatency device network library.");
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            var jni = m_DeviceNetworkLibrary.QueryInterface<AndroidJniWrapper.IAndroidJni>();
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
                return;
            }

            // Create environment from the code.
            m_Environment = m_EnvironmentSelectorLibrary.createEnvironment("AAVSaWdpZDEBBnllbGxvdwQDuFOJP9xGoD6vjmO9mpmZPgAAAAAAAAAAAJqZGT8DAgICAQEAAgACJFk");
            if (m_Environment == null)
            {
                Debug.LogError("Failed to create Antilatency environment");
                return;
            }

            // Get user node
            Antilatency.DeviceNetwork.NodeHandle[] compatibleUserNodes = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User");
            m_UserNodeHandle = compatibleUserNodes.Length > 0 ? compatibleUserNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_UserNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogWarning("User node handle was null.");
            }

            // Get admin node
            Antilatency.DeviceNetwork.NodeHandle[] compatibleAdminNodes = GetIdleTrackerNodesBySocketTag("Admin");
            m_AdminNodeHandle = compatibleAdminNodes.Length > 0 ? compatibleAdminNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                Debug.LogWarning("Admin node handle was null.");
            }

            // Tracking
            m_DeviceNetworkChanged.AddListener(OnDeviceNetworkChanged);
            OnDeviceNetworkChanged();
        }

        private void Update()
        {
            // Invoke DeviceNetworkChanged, if changed
            uint updateId = m_NativeNetwork.getUpdateId();
            if (updateId != m_LastUpdateId)
            {
                m_LastUpdateId = updateId;
                m_DeviceNetworkChanged.Invoke();
            }

            // Stop tracking cotask, if task has finished
            if (m_UserTrackingCotask != null && m_UserTrackingCotask.isTaskFinished())
            {
                StopTracking();
                return;
            }

            // Update tracking result
            Antilatency.Alt.Tracking.State trackingState;
            if (GetTrackingState(out trackingState))
            {
                m_UserNodePosition = trackingState.pose.position;
                m_UserNodeRotation = trackingState.pose.rotation;
            }
        }

        private void FixedUpdate()
        {
            if (m_UserNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                // Get user node
                Antilatency.DeviceNetwork.NodeHandle[] compatibleUserNodes = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User");
                m_UserNodeHandle = compatibleUserNodes.Length > 0 ? compatibleUserNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
                if (m_UserNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
                {
                    Debug.LogWarning("User node handle was null.");
                }
            }

            if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
            {
                // Get admin node
                Antilatency.DeviceNetwork.NodeHandle[] compatibleAdminNodes = GetIdleTrackerNodesBySocketTag("Admin");
                m_AdminNodeHandle = compatibleAdminNodes.Length > 0 ? compatibleAdminNodes[0] : Antilatency.DeviceNetwork.NodeHandle.Null;
                if (m_AdminNodeHandle == Antilatency.DeviceNetwork.NodeHandle.Null)
                {
                    Debug.LogWarning("Admin node handle was null.");
                }
            }
        }

        private void OnDestroy()
        {
            // Terminate tracking
            if (m_NativeNetwork != null)
            {
                m_DeviceNetworkChanged.RemoveListener(OnDeviceNetworkChanged);
            }

            StopTracking();

            if (m_UserTrackingCotask != null)
            {
                m_UserTrackingCotask.Dispose();
                m_UserTrackingCotask = null;
            }

            if (m_TrackingLibrary != null)
            {
                m_TrackingLibrary.Dispose();
                m_TrackingLibrary = null;
            }

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

        #region Tracking

        public Vector3 GetUserPosition() { return m_UserNodePosition; } // TMP
        public Quaternion GetUserRotation() { return m_UserNodeRotation; } // TMP

        private void OnDeviceNetworkChanged()
        {
            if (m_UserTrackingCotask != null)
            {
                if (m_UserTrackingCotask.isTaskFinished())
                {
                    StopTracking();
                }
                else
                {
                    return;
                }
            }

            if (m_UserTrackingCotask == null)
            {
                var node = m_UserNodeHandle;
                if (node != Antilatency.DeviceNetwork.NodeHandle.Null)
                {
                    StartTracking(node);
                }
            }
        }

        private bool GetRawTrackingState(out Antilatency.Alt.Tracking.State state)
        {
            state = new Antilatency.Alt.Tracking.State();
            if (m_UserTrackingCotask == null)
            {
                return false;
            }

            state = m_UserTrackingCotask.getState(Antilatency.Alt.Tracking.Constants.DefaultAngularVelocityAvgTime);
            if (state.stability.stage == Antilatency.Alt.Tracking.Stage.InertialDataInitialization)
            {
                return false;
            }
            return true;
        }

        private bool GetTrackingState(out Antilatency.Alt.Tracking.State state)
        {
            state = new Antilatency.Alt.Tracking.State();
            if (m_UserTrackingCotask == null)
            {
                return false;
            }

            state = m_UserTrackingCotask.getExtrapolatedState(m_TrackingPlacement, m_UserTrackingExtrapolationTime);
            if (state.stability.stage == Antilatency.Alt.Tracking.Stage.InertialDataInitialization)
            {
                return false;
            }
            return true;
        }

        private void StartTracking(Antilatency.DeviceNetwork.NodeHandle node)
        {
            if (m_NativeNetwork == null)
            {
                Debug.LogError("Native network was null.");
                return;
            }

            if (m_NativeNetwork.nodeGetStatus(node) != Antilatency.DeviceNetwork.NodeStatus.Idle)
            {
                Debug.LogError("Tracking node has wrong node status.");
                return;
            }

            if (m_Environment == null)
            {
                Debug.LogError("Environment was null.");
                return;
            }

            m_TrackingPlacement = GetPlacement();

            using (var cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                m_UserTrackingCotask = cotaskConstructor.startTask(m_NativeNetwork, node, m_Environment);

                if (m_UserTrackingCotask == null)
                {
                    StopTracking();
                    Debug.LogWarning("Failed to start tracking task on node " + node.value);
                    return;
                }
            }
        }

        private void StopTracking()
        {
            if (m_UserTrackingCotask == null)
            {
                return;
            }

            m_UserTrackingCotask.Dispose();
            m_UserTrackingCotask = null;
        }

        private Pose GetPlacement()
        {
            Debug.LogWarning("TODO: FIX ME!");
            return Pose.identity;
        }

        #endregion

        #region Storage

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

        #endregion

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
