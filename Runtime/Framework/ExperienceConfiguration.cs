using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{

    [CreateAssetMenu(fileName = "NewExperienceConfiguration", menuName = "MUCOSDK/Experience Configuration")]
    public class ExperienceConfiguration : ScriptableObject
    {
        public string Name;
        public string Description;

        public SceneAsset Scene;

        public GameObject LocalUserPrefab;
        public GameObject RemoteUserPrefab;
    }
}

