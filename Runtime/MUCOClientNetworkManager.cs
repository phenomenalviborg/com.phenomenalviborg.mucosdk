using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOClientNetworkManager : MonoBehaviour
    {
        [HideInInspector] public static MUCOClientNetworkManager Instance { get; private set; } = null;
        [HideInInspector] public MUCOClient Client { get; private set; } = null;

        [Header("Networking")]
        [SerializeField] private string m_ServerAddress = "127.0.0.1";
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
                Debug.LogError("MUCOClientNetworkManager is a singleton, multiple instances are not supported!");
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Client = new MUCOClient();
            Client.RegisterPacketHandler((int)MUCOServerPackets.SpawnUser, HandleSpawnUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.RemoveUser, HandleRemoveUser);
            Client.RegisterPacketHandler((int)MUCOServerPackets.TranslateUser, HandleTranslateUser);
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
        private void HandleSpawnUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                Debug.Log($"User Connected: {userID}");

                m_UserObjects[userID] = Instantiate(m_UserPrefab);

                MUCOUser user = m_UserObjects[userID].GetComponent<MUCOUser>();
                user.Initialize(userID, false);
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
        #endregion
    }
}