using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class User : MonoBehaviour
    {
        public int UserIdentifier = -1;
        public bool IsLocalUser = false;

        [Header("Replication")]
        private Vector3 m_LastReplicatedPosition = Vector3.zero;
        private Vector3 m_LastReplicatedRotation = Vector3.zero;

        private static bool s_StaticallyInitialized = false;
        private static Dictionary<int, User> s_Users = new Dictionary<int, User>();

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;

            if (s_StaticallyInitialized)
            {
                ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((int)EPacketIdentifier.MulticastTranslateUser, HandleMulticastTranslateUser);
                ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((int)EPacketIdentifier.MulticastRotateUser, HandleMulticastRotateUser);
                s_StaticallyInitialized = true;
            }

            if (isLocalUser)
            {
                gameObject.SetActive(false); // To avoid awake on initialization

                // Camera
                gameObject.AddComponent<Camera>();

                gameObject.SetActive(true);
            }

            s_Users[UserIdentifier] = this;
        }

        public void FixedUpdate()
        {
            if (IsLocalUser)
            {
                // Replicate position
                if (m_LastReplicatedPosition != transform.position)
                {
                    MUCOPacket packet = new MUCOPacket((int)EPacketIdentifier.MulticastTranslateUser);
                    packet.WriteInt(UserIdentifier);
                    packet.WriteFloat(transform.position.x);
                    packet.WriteFloat(transform.position.y);
                    packet.WriteFloat(transform.position.z);
                    ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);

                    m_LastReplicatedPosition = transform.position;
                }

                // Replicate rotation
                if (m_LastReplicatedRotation != transform.rotation.eulerAngles)
                {
                    MUCOPacket packet = new MUCOPacket((int)EPacketIdentifier.MulticastRotateUser);
                    packet.WriteInt(UserIdentifier);
                    packet.WriteFloat(transform.rotation.eulerAngles.x);
                    packet.WriteFloat(transform.rotation.eulerAngles.y);
                    packet.WriteFloat(transform.rotation.eulerAngles.z);
                    ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);

                    m_LastReplicatedRotation = transform.rotation.eulerAngles;
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Sending packet...");
                MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.TranslateUser);
                packet.WriteFloat(1.0f);
                packet.WriteFloat(1.0f);
                packet.WriteFloat(1.0f);

                ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);
            }
        }

        public static void HandleMulticastTranslateUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                float positionX = packet.ReadFloat();
                float positionY = packet.ReadFloat();
                float positionZ = packet.ReadFloat();

                s_Users[userID].transform.position = new Vector3(positionX, positionY, positionZ);
            });
        }

        public static void HandleMulticastRotateUser(MUCOPacket packet)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                int userID = packet.ReadInt();
                float eulerAnglesX = packet.ReadFloat();
                float eulerAnglesY = packet.ReadFloat();
                float eulerAnglesZ = packet.ReadFloat();

                s_Users[userID].transform.rotation = Quaternion.Euler(new Vector3(eulerAnglesX, eulerAnglesY, eulerAnglesZ));
            });
        }
    }
}
