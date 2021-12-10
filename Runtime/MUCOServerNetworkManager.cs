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
        [SerializeField] private GameObject m_UserPrefab = null;

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Server = new MUCOServer();
            Server.Start(m_ServerPort);
            Server.OnClientConnectedEvent += OnClientConnected;
            Server.OnClientDisconnectedEvent += OnClientDisconnected;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.SpawnUser);
                packet.WriteInt(999);
                Server.SendPacketToAll(packet);
            }
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        private void OnClientConnected(MUCOServer.MUCOClientInfo newClientInfo)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                // Create a user object on the server.
                Debug.Log($"User Connected: {newClientInfo}");
                m_UserObjects[newClientInfo.UniqueIdentifier] = Instantiate(m_UserPrefab);

                // Update the newly connected user about all the other users in existance.
                foreach (MUCOServer.MUCOClientInfo clientInfo in Server.ClientInfo.Values)
                {
                    if (clientInfo.UniqueIdentifier == newClientInfo.UniqueIdentifier)
                    {
                        continue;
                    }

                    MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.SpawnUser);
                    packet.WriteInt(clientInfo.UniqueIdentifier);
                    Server.SendPacket(newClientInfo, packet);
                }

                // Spawn the new user on all clients (includeing the new client).
                foreach (MUCOServer.MUCOClientInfo clientInfo in Server.ClientInfo.Values)
                {
                    MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.SpawnUser);
                    packet.WriteInt(newClientInfo.UniqueIdentifier);
                    Server.SendPacket(clientInfo, packet);
                }
            });
        }

        private void OnClientDisconnected(MUCOServer.MUCOClientInfo disconnectingClientInfo)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                // Destroy disconnected users game object.
                Debug.Log($"User Disconnected: {disconnectingClientInfo}");
                Destroy(m_UserObjects[disconnectingClientInfo.UniqueIdentifier]);
                m_UserObjects[disconnectingClientInfo.UniqueIdentifier] = null;

                // Remove the disconnecting user on all clients (includeing the new client).
                foreach (MUCOServer.MUCOClientInfo clientInfo in Server.ClientInfo.Values)
                {
                    if (clientInfo.UniqueIdentifier == disconnectingClientInfo.UniqueIdentifier)
                    {
                        continue;
                    }

                    MUCOPacket packet = new MUCOPacket((int)MUCOServerPackets.RemoveUser);
                    packet.WriteInt(disconnectingClientInfo.UniqueIdentifier);
                    Server.SendPacket(clientInfo, packet);
                }
            });
        }

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }
    }
}