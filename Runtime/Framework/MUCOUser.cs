using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOUser : MonoBehaviour
    {
        [Header("Replication")]
        private Vector3 m_LastReplicatedPosition = Vector3.zero;
        private Vector3 m_LastReplicatedRotation = Vector3.zero;

        // TODO: Custom data type
        public int UserIdentifier /*{ get; private set; }*/ = -1;
        public bool IsLocalUser = false;

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;

            InvokeRepeating("SendDeviceInfo", 0.0f, 1.0f);

            if (isLocalUser)
            {
                gameObject.AddComponent<Camera>();
            }
        }

        public void FixedUpdate()
        {
            if (IsLocalUser)
            {
                // Replicate position
                if (m_LastReplicatedRotation != transform.position)
                {
                    MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.TranslateUser);
                    packet.WriteFloat(transform.position.x);
                    packet.WriteFloat(transform.position.y);
                    packet.WriteFloat(transform.position.z);
                    MUCOClientNetworkManager.GetInstance().Client.SendPacket(packet);

                    m_LastReplicatedPosition = transform.position;
                }

                // Replicate rotation
                if (m_LastReplicatedRotation != transform.rotation.eulerAngles)
                {
                    MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.RotateUser);
                    packet.WriteFloat(transform.rotation.eulerAngles.x);
                    packet.WriteFloat(transform.rotation.eulerAngles.y);
                    packet.WriteFloat(transform.rotation.eulerAngles.z);
                    MUCOClientNetworkManager.GetInstance().Client.SendPacket(packet);

                    m_LastReplicatedRotation = transform.rotation.eulerAngles;
                }
            }
        }

        private void SendDeviceInfo()
        {
            if (IsLocalUser)
            {
                MUCOClientNetworkManager.GetInstance().SendDeviceInfo();
            }
        }
    }
}