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
        public SceneAsset EntryScene;
        public SceneAsset MenuScene;

        public List<ExperienceConfiguration> ExperienceConfigurations = new List<ExperienceConfiguration>();
    }
}

