using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOUser : MonoBehaviour
    {

        [Header("Replication")]
        [SerializeField] private float m_MinimumTransformUpdateDelta = 0.1f;
        [SerializeField] private Vector3 m_LastReplicatedPosition = Vector3.zero;

        // TODO: Custom data type
        public int UserIdentifier /*{ get; private set; }*/ = -1;
        public bool IsLocalUser = false;

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;
        }

        public void Update()
        {
            if (IsLocalUser && Vector3.Distance(m_LastReplicatedPosition, transform.position) > m_MinimumTransformUpdateDelta)
            {
                m_LastReplicatedPosition = transform.position;

                MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.TranslateUser);
                packet.WriteFloat(transform.position.x);
                packet.WriteFloat(transform.position.y);
                packet.WriteFloat(transform.position.z);
                MUCOClientNetworkManager.Instance.Client.SendPacket(packet);
            }
        }
    }
}