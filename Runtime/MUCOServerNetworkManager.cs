using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOServerNetworkManager : MonoBehaviour
    {
        [HideInInspector] public static MUCOServerNetworkManager Instance { get; private set; } = null;
        [HideInInspector] public MUCOServer Server { get; private set; } = null;

        [Header("Networking")]
        [SerializeField] private int m_ServerPort = 1000;

        [Header("Debug")]
        [SerializeField] private MUCOLogMessage.MUCOLogLevel m_LogLevel = MUCOLogMessage.MUCOLogLevel.Info;

        [Header("This should not stay in this class!")]
        [SerializeField] private Dictionary<int, GameObject> m_UserObjects = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject m_UserPrefab = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                Debug.LogError("MUCOServerNetworkManager is a singleton, multiple instances are not supported!");
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Server = new MUCOServer();
            Server.RegisterPacketHandler((int)MUCOClientPackets.TranslateUser, HandleTranslateUser);
            Server.RegisterPacketHandler((int)MUCOClientPackets.RotateUser, HandleRotateUser);
            Server.RegisterPacketHandler((int)MUCOClientPackets.DeviceInfo, HandleDeviceInfo);
            Server.OnClientConnectedEvent += OnClientConnected;
            Server.OnClientDisconnectedEvent += OnClientDisconnected;
            Server.Start(m_ServerPort);
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
                MUCOUser user = m_UserObjects[newClientInfo.UniqueIdentifier].GetComponent<MUCOUser>();
                user.Initialize(newClientInfo.UniqueIdentifier, false);

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

        # region Packet handlers
        private void HandleTranslateUser(MUCOPacket packet, int fromClient)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                // Update the clinet position locally on the server.
                float positionX = packet.ReadFloat();
                float positionY = packet.ReadFloat();
                float positionZ = packet.ReadFloat();
                m_UserObjects[fromClient].transform.position = new Vector3(positionX, positionY, positionZ);

                // Replicate the packet to the other clients.
                MUCOPacket replicatePacket = new MUCOPacket((int)MUCOServerPackets.TranslateUser);
                replicatePacket.WriteInt(fromClient);
                replicatePacket.WriteFloat(positionX);
                replicatePacket.WriteFloat(positionY);
                replicatePacket.WriteFloat(positionZ);
                Server.SendPacketToAllExceptOne(replicatePacket, Server.ClientInfo[fromClient]);
            });
        }

        private void HandleRotateUser(MUCOPacket packet, int fromClient)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                // Update the clinet rotation locally on the server.
                float eulerAnglesX = packet.ReadFloat();
                float eulerAnglesY = packet.ReadFloat();
                float eulerAnglesZ = packet.ReadFloat();

                m_UserObjects[fromClient].transform.rotation = Quaternion.Euler(new Vector3(eulerAnglesX, eulerAnglesY, eulerAnglesZ));

                // Replicate the packet to the other clients.
                MUCOPacket replicatePacket = new MUCOPacket((int)MUCOServerPackets.RotateUser);
                replicatePacket.WriteInt(fromClient);
                replicatePacket.WriteFloat(eulerAnglesX);
                replicatePacket.WriteFloat(eulerAnglesY);
                replicatePacket.WriteFloat(eulerAnglesZ);
                Server.SendPacketToAllExceptOne(replicatePacket, Server.ClientInfo[fromClient]);
            });
        }

        private void HandleDeviceInfo(MUCOPacket packet, int fromClient)
        {

        }
        #endregion

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }
    }
}