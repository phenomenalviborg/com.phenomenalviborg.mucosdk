using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{
    public enum EUserPacketIdentifier : System.UInt32
    {
        // User related 1000-1100
        MulticastTransformUserRoot = 1000,
        MulticastTransformUserHead = 1001,
        MulticastTransformUserLeftHand = 1002,
        MulticastTransformUserRightHand = 1003,
    }

    public class User : MonoBehaviour
    {
        public int UserIdentifier = -1;
        public bool IsLocalUser = false;

        public Transform headTransform;
        public Transform leftHandTransform;
        public Transform rightHandTransform;

        private static bool s_StaticallyInitialized = false;
        private static Dictionary<int, User> s_Users = new Dictionary<int, User>();

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;

            if (!s_StaticallyInitialized)
            {
                ClientNetworkManager.GetInstance().RegisterPacketHandler((System.UInt16)EUserPacketIdentifier.MulticastTransformUserRoot, HandleMulticastTransformUserRoot);
                ClientNetworkManager.GetInstance().RegisterPacketHandler((System.UInt16)EUserPacketIdentifier.MulticastTransformUserHead, HandleMulticastTransformUserHead);
                ClientNetworkManager.GetInstance().RegisterPacketHandler((System.UInt16)EUserPacketIdentifier.MulticastTransformUserLeftHand, HandleMulticastTransformUserLeftHand);
                ClientNetworkManager.GetInstance().RegisterPacketHandler((System.UInt16)EUserPacketIdentifier.MulticastTransformUserRightHand, HandleMulticastTransformUserRightHand);
                s_StaticallyInitialized = true;
            }

            s_Users[UserIdentifier] = this;
        }

        private void FixedUpdate()
        {
            if (IsLocalUser)
            {
                if (true)
                {
                    MulticastTransformHelper(transform, (System.UInt16)EUserPacketIdentifier.MulticastTransformUserRoot);
                }

                if (true)
                {
                    MulticastTransformHelper(headTransform, (System.UInt16)EUserPacketIdentifier.MulticastTransformUserHead);
                }

                if (true)
                {
                    MulticastTransformHelper(leftHandTransform, (System.UInt16)EUserPacketIdentifier.MulticastTransformUserLeftHand);
                }

                if (true)
                {
                    MulticastTransformHelper(rightHandTransform, (System.UInt16)EUserPacketIdentifier.MulticastTransformUserRightHand);
                }
            }
        }

        private static void HandleMulticastTransformUserRoot(MUCOPacket packet)
        {
            int userID = packet.ReadInt();
            User user = s_Users[userID];

            if (user)
            {
                Vector3 position;
                Quaternion rotation;
                HandleMulticastTransformHelper(packet, out position, out rotation);
                user.transform.position = position;
                user.transform.rotation = rotation;
            }
        }
        private static void HandleMulticastTransformUserHead(MUCOPacket packet)
        {
            int userID = packet.ReadInt();
            User user = s_Users[userID];

            if (user)
            {
                Vector3 position;
                Quaternion rotation;
                HandleMulticastTransformHelper(packet, out position, out rotation);
                user.headTransform.position = position;
                user.headTransform.rotation = rotation;
            }
        }
        private static void HandleMulticastTransformUserLeftHand(MUCOPacket packet)
        {
            int userID = packet.ReadInt();
            User user = s_Users[userID];

            if (user)
            {
                Vector3 position;
                Quaternion rotation;
                HandleMulticastTransformHelper(packet, out position, out rotation);
                user.leftHandTransform.position = position;
                user.leftHandTransform.rotation = rotation;
            }
        }
        private static void HandleMulticastTransformUserRightHand(MUCOPacket packet)
        {
            int userID = packet.ReadInt();
            User user = s_Users[userID];

            if (user)
            {
                Vector3 position;
                Quaternion rotation;
                HandleMulticastTransformHelper(packet, out position, out rotation);
                user.rightHandTransform.position = position;
                user.rightHandTransform.rotation = rotation;
            }
        }

        private void MulticastTransformHelper(Transform transform, System.UInt16 packetIdentifier)
        {
            using (MUCOPacket packet = new MUCOPacket(packetIdentifier))
            {
                packet.WriteInt(UserIdentifier);
                packet.WriteFloat(transform.position.x);
                packet.WriteFloat(transform.position.y);
                packet.WriteFloat(transform.position.z);
                packet.WriteFloat(transform.rotation.x);
                packet.WriteFloat(transform.rotation.y);
                packet.WriteFloat(transform.rotation.z);

                ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);
            }
        }

        private static void HandleMulticastTransformHelper(MUCOPacket packet, out Vector3 position, out Quaternion rotation)
        {
            float positionX = packet.ReadFloat();
            float positionY = packet.ReadFloat();
            float positionZ = packet.ReadFloat();
            float eulerAnglesX = packet.ReadFloat();
            float eulerAnglesY = packet.ReadFloat();
            float eulerAnglesZ = packet.ReadFloat();

            position = new Vector3(positionX, positionY, positionZ);
            rotation = Quaternion.Euler(new Vector3(eulerAnglesX, eulerAnglesY, eulerAnglesZ));
        }
    }
}
