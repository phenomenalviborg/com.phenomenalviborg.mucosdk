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

        [Header("Debug")]
        [SerializeField] private MUCOLogMessage.MUCOLogLevel m_LogLevel = MUCOLogMessage.MUCOLogLevel.Info;

        [Header("This should not stay in this class!")]
        [SerializeField] private Dictionary<int, GameObject> m_UserObjects = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject m_UserPrefab = null;

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

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

            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();

                Debug.Log($"User Connected: {userID}");

                m_UserObjects[userID] = Instantiate(m_UserPrefab, new Vector3(userID, 0.0f, 0.0f), Quaternion.identity);
            });
        }

        private void HandleUserDisconnected(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();

                Debug.Log($"User Disconnected: {userID}");

                Destroy(m_UserObjects[userID]);
                m_UserObjects[userID] = null;
            });
        }
        #endregion
    }
}