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

        public bool ManualInitialization = false;
    }
}

