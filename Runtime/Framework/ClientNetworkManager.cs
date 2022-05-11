using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;


namespace PhenomenalViborg.MUCOSDK
{
    public class ClientNetworkManager : PhenomenalViborg.MUCOSDK.IManager<ClientNetworkManager>
    {
        [HideInInspector] public MUCOClient Client { get; private set; } = null;

        [Header("Networking")]
        [SerializeField] private string m_ServerAddress = "127.0.0.1";
        [SerializeField] private int m_ServerPort = 1000;

        [Header("Debug")]
        [SerializeField] private MUCOLogMessage.MUCOLogLevel m_LogLevel = MUCOLogMessage.MUCOLogLevel.Info;

        [Header("This should not stay in this class!")]
        [SerializeField] private Dictionary<int, GameObject> m_UserObjects = new Dictionary<int, GameObject>();
        [SerializeField] private GameObject m_RemoteUserPrefab = null;
        [SerializeField] private GameObject m_LocalUserPrefab = null;

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Client = new MUCOClient();
            Client.RegisterPacketHandler((int)MUCOServerPackets.SpawnUser, HandleSpawnUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.RemoveUser, HandleRemoveUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.TranslateUser, HandleTranslateUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.RotateUser, HandleRotateUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.LoadExperience, HandleLoadExperience);
            Client.Connect(m_ServerAddress, m_ServerPort);
        }

        public void Connect(string address, int port)
        {
            Debug.Log($"Connecting: {address}:{port}");
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
        private void HandleSpawnUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                Debug.Log($"User Connected: {userID}");

                bool localUser = userID == Client.UniqueIdentifier;

                m_UserObjects[userID] = Instantiate(localUser ? m_LocalUserPrefab : m_RemoteUserPrefab);
                DontDestroyOnLoad(m_UserObjects[userID]);
                User user = m_UserObjects[userID].GetComponent<User>();
                user.Initialize(userID, userID == Client.UniqueIdentifier);
            });
        }

        private void HandleRemoveUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                Debug.Log($"User Disconnected: {userID}");

                Destroy(m_UserObjects[userID]);
                m_UserObjects[userID] = null;
            });
        }

        private void HandleTranslateUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                float positionX = packet.ReadFloat();
                float positionY = packet.ReadFloat();
                float positionZ = packet.ReadFloat();

                m_UserObjects[userID].transform.position = new Vector3(positionX, positionY, positionZ);
            });
        }

        private void HandleRotateUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                float eulerAnglesX = packet.ReadFloat();
                float eulerAnglesY = packet.ReadFloat();
                float eulerAnglesZ = packet.ReadFloat();

                m_UserObjects[userID].transform.rotation = Quaternion.Euler(new Vector3(eulerAnglesX, eulerAnglesY, eulerAnglesZ));
            });
        }

        private void HandleLoadExperience(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                string experienceName = packet.ReadString();
                Debug.Log($"HandleLoadExperience - {experienceName}");
                ApplicationManager.GetInstance().LoadExperienceByName(experienceName);
            });
        }
        #endregion

        #region Packet senders
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
        }
        #endregion
    }
}