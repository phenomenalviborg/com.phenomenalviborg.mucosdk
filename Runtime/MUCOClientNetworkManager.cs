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

        private void Start()
        {
            MUCOLogger.LogEvent += Log;

            Client = new MUCOClient();
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
    }
}