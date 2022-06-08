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
        public bool UseStaticServerInfo = false;
        public string ServerAddress = "127.0.0.1";
        public UInt16 ServerPort = 4960;
    }
}

