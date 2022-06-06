using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;


namespace PhenomenalViborg.MUCOSDK
{
    public struct NetworkUser
    {
        public int Identifier;
        public bool IsLocalUser;
    }

    public class ClientNetworkManager : PhenomenalViborg.MUCOSDK.IManager<ClientNetworkManager>
    {
        [HideInInspector] public MUCOClient Client { get; private set; } = null;

        private List<NetworkUser> m_NetworkUsers = new List<NetworkUser>();

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = MUCOLogMessage.MUCOLogLevel.Warn;

            Client = new MUCOClient();
            Client.RegisterPacketHandler((System.UInt16)EPacketIdentifier.ServerUserConnected, HandleUserConnected);
            Client.RegisterPacketHandler((System.UInt16)EPacketIdentifier.ServerUserDisconnected, HandleUserDisconnected);
            Client.RegisterPacketHandler((System.UInt16)EPacketIdentifier.ServerLoadExperience, HandleLoadExperience);
        }

        public void Connect(string address, int port)
        {
            Debug.Log($"Connecting: {address}:{port}");
            Client.Connect(address, port);
        }

        // This basically only exists because of offline mode, and should not ideally be used for other purposes.
        public void AddNetworkUser(NetworkUser networkUser)
        {
            m_NetworkUsers.Add(networkUser);
        }

        public void Disconnect()
        {
            Client.Disconnect();
        }

        private void OnApplicationQuit()
        {
            // TODO: If connected
            Disconnect();
        }

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }

        public List<NetworkUser> GetNetworkUsers()
        {
            return m_NetworkUsers;
        }

        # region Packet handlers
        private void HandleUserConnected(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                NetworkUser networkUser;
                networkUser.Identifier = packet.ReadInt();
                Debug.Log($"User Connected: {networkUser.Identifier}");
                networkUser.IsLocalUser = (networkUser.Identifier == Client.UniqueIdentifier);
                m_NetworkUsers.Add(networkUser);

            });
        }
        private void HandleUserDisconnected(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userIdentifier = packet.ReadInt();
                Debug.Log($"User Disconnected: {userIdentifier}");

                NetworkUser? networkUser = m_NetworkUsers.Find(user => user.Identifier == userIdentifier);
                if (networkUser != null)
                {
                    m_NetworkUsers.Remove((NetworkUser)networkUser);
                }
            });
        }
        private void HandleLoadExperience(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                string experienceName = packet.ReadString();
                Debug.Log($"HandleLoadExperience - {experienceName}");
                MUCOApplication.GetApplicationManager().LoadExperienceByName(experienceName);
            });
        }
        #endregion

        #region Packet senders
        public void SendReplicatedUnicastPacket(MUCOPacket packet, NetworkUser receiver)
        {
            using (MUCOPacket unicastPacket = new MUCOPacket((System.UInt16)EPacketIdentifier.ClientGenericReplicatedUnicast))
            {
                packet.SetReadOffset(0);
                unicastPacket.WriteInt((int)receiver.Identifier);
                unicastPacket.WriteBytes(packet.ReadBytes(packet.GetSize()));
                Client.SendPacket(unicastPacket);
            }
        }

        public void SendReplicatedMulticastPacket(MUCOPacket packet)
        {
            using (MUCOPacket multicastPacket = new MUCOPacket((System.UInt16)EPacketIdentifier.ClientGenericReplicatedMulticast))
            {
                packet.SetReadOffset(0);
                multicastPacket.WriteBytes(packet.ReadBytes(packet.GetSize()));
                Client.SendPacket(multicastPacket);
            }
        }

        /*// TODO: InvokeRepeating("SendDeviceInfo", 0.0f, 1.0f);
        public void SendDeviceInfo()
        {
            Debug.Log("Battery level: " + SystemInfo.batteryLevel);
            Debug.Log("Battery state: " + SystemInfo.batteryStatus);
            Debug.Log("Device model: " + SystemInfo.deviceModel);
            Debug.Log("Device UUID: " + SystemInfo.deviceUniqueIdentifier);
            Debug.Log("Device OS: " + SystemInfo.operatingSystem);

            using (MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.DeviceInfo))
            {
                packet.WriteFloat(SystemInfo.batteryLevel);
                packet.WriteInt((int)SystemInfo.batteryStatus);
                packet.WriteString(SystemInfo.deviceModel);
                packet.WriteString(SystemInfo.deviceUniqueIdentifier);
                packet.WriteString(SystemInfo.operatingSystem);
                Client.SendPacket(packet);
            }
        }*/
        #endregion
    }
}