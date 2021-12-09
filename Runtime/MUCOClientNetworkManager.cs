using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOClientNetworkManager : MonoBehaviour
    {
        [HideInInspector] public MUCOClient Client { get; private set; } = null;

        [Header("Networking")]
        [SerializeField] private string m_ServerAddress = "127.0.0.1";
        [SerializeField] private int m_ServerPort = 1000;

        [Header("This should not stay in this class!")]
        [SerializeField] private Dictionary<int, GameObject> m_UserObjects = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject m_MUCOUserPrefab = null;

        private void Start()
        {
            MUCOLogger.LogEvent += Log;

            Client = new MUCOClient();
            Client.RegisterPacketHandler((int)MUCOServerPackets.UserConnected, HandleUserConnected);
            Client.RegisterPacketHandler((int)MUCOServerPackets.UserDisconnected, HandleUserDisconnected);
            Client.Connect(m_ServerAddress, m_ServerPort);
        }

        private void OnApplicationQuit()
        {
            Client.Disconnect();
        }

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }

        # region Packet handlers
        private void HandleUserConnected(MUCOPacket packet)
        {
            int userID = packet.ReadInt();

            Debug.Log($"User Connected: {userID}");

            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                m_UserObjects[userID] = Instantiate(m_MUCOUserPrefab);
            });
        }

        private void HandleUserDisconnected(MUCOPacket packet)
        {
            int userID = packet.ReadInt();

            Debug.Log($"User Disconnected: {userID}");

            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                Destroy(m_UserObjects[userID]);
                m_UserObjects[userID] = null;
            });
        }
        #endregion
    }
}