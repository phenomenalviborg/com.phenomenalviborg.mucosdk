using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class ExperienceManager : PhenomenalViborg.MUCOSDK.IManager<ExperienceManager>
    {
        [SerializeField] private ExperienceConfiguration m_ExperienceConfiguration;

        Dictionary<NetworkUser, GameObject> m_UserGameObjects = new Dictionary<NetworkUser, GameObject>();

        public void Initialize(ExperienceConfiguration experienceConfiguration)
        {
            m_ExperienceConfiguration = experienceConfiguration;

            /*// Spawn connected users
            foreach (NetworkUser networkUser in ClientNetworkManager.GetInstance().GetNetworkUsers())
            {
                SpawnUser(networkUser);
            }*/
        }

        public void SpawnUser(NetworkUser networkUser)
        {
            if (m_UserGameObjects.ContainsKey(networkUser))
            {
                Debug.LogError($"UserGameObjects already contains {networkUser}");
                return;
            }

            GameObject userPrefab = networkUser.IsLocalUser ? m_ExperienceConfiguration.LocalUserPrefab : m_ExperienceConfiguration.RemoteUserPrefab;
            GameObject userGameObject = Instantiate(userPrefab);
            User user = userGameObject.GetComponent<User>();
            user.Initialize(networkUser.Identifier, networkUser.IsLocalUser);
            DontDestroyOnLoad(userGameObject);

            m_UserGameObjects[networkUser] = userGameObject;
        }

        public void RemoveUser(NetworkUser networkUser)
        {
            GameObject userGameObject = m_UserGameObjects[networkUser];
            if (userGameObject == null)
            {
                Debug.Log($"Failed to find user '{networkUser.Identifier}'");
                return;
            }

            m_UserGameObjects.Remove(networkUser);
            Destroy(userGameObject);
        }
    }
}
