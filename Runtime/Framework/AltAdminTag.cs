using System.Linq;

using UnityEngine;

using Antilatency.DeviceNetwork;

namespace PhenomenalViborg.MUCOSDK
{
    public class AltAdminTag : MonoBehaviour
    {
        private Antilatency.SDK.DeviceNetwork m_DeviceNetwork;
        private Antilatency.SDK.AltEnvironmentCode m_Environment;

        private Antilatency.Alt.Tracking.ILibrary m_TrackingLibrary;
        private Antilatency.DeviceNetwork.NodeHandle m_Node;

        private void Start()
        {
            // m_DeviceNetwork = GetComponent<ApplicationManager>().GetDeviceNetwork();
            // m_Environment = GetComponent<ApplicationManager>().GetEnvironment();

            m_Node = new Antilatency.DeviceNetwork.NodeHandle();

            if (m_DeviceNetwork == null)
            {
                Debug.LogError("Network is null");
                return;
            }

            m_TrackingLibrary = Antilatency.Alt.Tracking.Library.load();

            if (m_TrackingLibrary == null)
            {
                Debug.LogError("Failed to create tracking library");
                return;

            }
            m_Node = GetFirstIdleTrackerNodeBySocketTag("Admin");
        }

        private void Update()
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = GetNativeNetwork();
            Debug.Log(nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(m_Node), "ServerAddress"));
        }

        // ALT
        protected INetwork GetNativeNetwork()
        {
            if (m_DeviceNetwork == null)
            {
                Debug.LogError("Network is null");
                return null;
            }

            if (m_DeviceNetwork == null)
            {
                Debug.LogError("Native network is null");
                return null;
            }

            return m_DeviceNetwork.NativeNetwork;
        }


        #region Usefull methods to search nodes

        /// <summary>
        /// Searches for all idle tracker nodes.
        /// </summary>
        /// <returns>The array of currently idle tracker nodes.</returns>
        protected NodeHandle[] GetIdleTrackerNodes()
        {
            var nativeNetwork = GetNativeNetwork();

            if (nativeNetwork == null)
            {
                return new NodeHandle[0];
            }

            using (var cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                    ).ToArray();

                return nodes;
            }
        }

        /// <summary>
        /// Searches for the first idle tracking node.
        /// </summary>
        /// <returns>The first idle tracking node if it exists, otherwise NodeHandle with value = -1 (InvalidNode).</returns>
        protected NodeHandle GetFirstIdleTrackerNode()
        {
            var nodes = GetIdleTrackerNodes();
            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }
            return nodes[0];
        }

        /// <summary>
        /// Searches for all idle tracker nodes that are directly connected to USB socket.
        /// </summary>
        /// <returns>The array of idle tracker nodes connected directly to USB sockets.</returns>
        protected NodeHandle[] GetUsbConnectedIdleTrackerNodes()
        {
            var nativeNetwork = GetNativeNetwork();

            if (nativeNetwork == null)
            {
                return new NodeHandle[0];
            }

            using (var cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetParent(nativeNetwork.nodeGetParent(v)) == Antilatency.DeviceNetwork.NodeHandle.Null &&
                        nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        /// <summary>
        /// Get the first idle tracker node connected directly to USB socket.
        /// </summary>
        /// <returns>The first idle tracker node connected directly to USB socket if exists, otherwise NodeHandle with value = -1 (InvalidNode).</returns>
        protected NodeHandle GetUsbConnectedFirstIdleTrackerNode()
        {
            var nodes = GetUsbConnectedIdleTrackerNodes();

            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }

            return nodes[0];
        }

        /// <summary>
        /// Searches for idle tracking nodes which socket is marked with <paramref name="socketTag"/>.
        /// </summary>
        /// <param name="socketTag">Socket "tag" property value.</param>
        /// <returns>The array of idle tracking nodes connected to sockets marked with <paramref name="socketTag"/>.</returns>
        protected NodeHandle[] GetIdleTrackerNodesBySocketTag(string socketTag)
        {
            var nativeNetwork = GetNativeNetwork();

            if (nativeNetwork == null)
            {
                return new NodeHandle[0];
            }

            using (var cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        /// <summary>
        /// Searches for the idle tracking node which socket is marked with <paramref name="socketTag"/>.
        /// </summary>
        /// <param name="socketTag">Socket "tag" property value.</param>
        /// <returns>The first idle tracking nodes connected to socket marked with <paramref name="socketTag"/>.</returns>
        protected NodeHandle GetFirstIdleTrackerNodeBySocketTag(string socketTag)
        {
            var nodes = GetIdleTrackerNodesBySocketTag(socketTag);

            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }

            return nodes[0];
        }

        #endregion
    }
}
