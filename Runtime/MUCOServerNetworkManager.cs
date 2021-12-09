using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOServerNetworkManager : MonoBehaviour
    {
        [HideInInspector] public MUCOServer Server { get; private set; } = null;

        [Header("Networking")]
        [SerializeField] private int m_ServerPort = 1000;

        [Header("Debug")]
        [SerializeField] private MUCOLogMessage.MUCOLogLevel m_LogLevel = MUCOLogMessage.MUCOLogLevel.Info;

        [Header("This should not stay in this class!")]
        [SerializeField] private Dictionary<int, GameObject> m_UserObjects = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject m_MUCOUserPrefab = null;

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Server = new MUCOServer();
            Server.Start(m_ServerPort);
            Server.OnClientConnectedEvent += OnClientConnected;
            Server.OnClientDisconnectedEvent += OnClientDisconnected;
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        private void OnClientConnected(MUCOServer.MUCOClientInfo clientInfo)
        {
            Debug.Log($"User Connected: {clientInfo}");

            MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.UserConnected);
            packet.WriteInt(clientInfo.UniqueIdentifier);
            Server.SendPacketToAll(packet);

            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                m_UserObjects[clientInfo.UniqueIdentifier] = Instantiate(m_MUCOUserPrefab);
            });
        }

        private void OnClientDisconnected(MUCOServer.MUCOClientInfo clientInfo)
        {
            Debug.Log($"User Disconnected: {clientInfo}");

            MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.UserDisconnected);
            packet.WriteInt(clientInfo.UniqueIdentifier);
            Server.SendPacketToAll(packet);

            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                Destroy(m_UserObjects[clientInfo.UniqueIdentifier]);
                m_UserObjects[clientInfo.UniqueIdentifier] = null;
            });
        }

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }
    }
}