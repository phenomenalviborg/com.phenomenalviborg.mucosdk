using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CreateAssetMenu(fileName = "NewApplicationConfiguration", menuName = "MUCOSDK/Application Configuration")]
    public class ApplicationConfiguration : ScriptableObject
    {
        public enum ConnectionProcedure { Manual }

        public SceneReference EntryScene;
        public SceneReference MenuScene;
        public List<ExperienceConfiguration> ExperienceConfigurations = new List<ExperienceConfiguration>();

        // Tracking
        public string AdminNodeTag = "Admin";
        public string UserNodeTag = "User";

        // Initialization
        public bool ManualInitialization = false;
        public bool OfflineMode = false;

        // Network      
        public ConnectionProcedure connectionProcedure = ConnectionProcedure.Manual;
        public string serverAddress = "127.0.0.1";
        public UInt16 serverPort = 4960;
    }
}

