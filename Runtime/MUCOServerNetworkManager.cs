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
        
        private void Start()
        {
            MUCOLogger.LogEvent += Log;
            MUCOLogger.LogLevel = m_LogLevel;

            Server = new MUCOServer();
            Server.Start(m_ServerPort);
            Server.OnClientConnectedEvent += OnClientConnected;
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        private void OnClientConnected(MUCOServer.MUCOClientInfo clientInfo)
        {
            Debug.Log($"Client Connected: {clientInfo}");
        }

        private static void Log(MUCOLogMessage message)
        {
            Debug.Log(message.ToString());
        }
    }
}