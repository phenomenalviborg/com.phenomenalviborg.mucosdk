using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class ExperienceManager : PhenomenalViborg.MUCOSDK.IManager<ExperienceManager>
    {
        [SerializeField] private ExperienceConfiguration m_ExperienceConfiguration;
        
        public void Initialize(ExperienceConfiguration experienceConfiguration)
        {
            Debug.Log("EexperienceManager::Initialize");
            m_ExperienceConfiguration = experienceConfiguration;

            // Spawn connected users
            foreach (NetworkUser networkUser in ClientNetworkManager.GetInstance().GetNetworkUsers())
            {
                SpawnUser(networkUser);
            }
        }

        public void SpawnUser(NetworkUser networkUser)
        {
            GameObject userPrefab = networkUser.IsLocalUser ? m_ExperienceConfiguration.LocalUserPrefab : m_ExperienceConfiguration.RemoteUserPrefab;
            GameObject userGameObject = Instantiate(userPrefab);
            User user = userGameObject.GetComponent<User>();
            user.Initialize(networkUser.Identifier, networkUser.IsLocalUser);
        }
    }
}
