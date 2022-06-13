using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine; // TODO: REMOVE
using PhenomenalViborg.MUCOSDK;
using PhenomenalViborg.MUCONet;
using System.Net;

namespace PhenomenalViborg.Networking
{
    public class NetworkIdentity
    {
        public NetworkIdentity(int networkIdentifier)
        {
            NetworkIdentifier = networkIdentifier;
        }

        public int NetworkIdentifier;
    }

    public class QuickAndDirtyReplicatedVariable<T>
    {
        private NetworkIdentity m_NetworkIdentity;
        public NetworkIdentity GetNetworkIdentity() { return m_NetworkIdentity; }

        public T Data;
        
        private static System.UInt16 m_IncrementalIdentifier;
        private System.UInt16 m_VariableIdentifier;

        private bool m_StaticallyInitialized = false;

        public QuickAndDirtyReplicatedVariable(T data, NetworkIdentity networkIdentity)
        {
            this.Data = data;
            this.m_NetworkIdentity = networkIdentity;
            this.m_VariableIdentifier = m_IncrementalIdentifier;

            m_IncrementalIdentifier++;
        
            if (!m_StaticallyInitialized)
            {
                ClientNetworkManager.GetInstance().RegisterPacketHandler((System.UInt16)EPacketIdentifier.MulticastReplicateGenericVariable, HandleMulticastReplicateGenericVariable);
                m_StaticallyInitialized = true;
            }
        }

        public T Get()
        {
            return Data;
        }

        public static byte[] NetSerializeToByteArray(T data)
        {
            if (data.GetType() == typeof(int))
            {
                byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(data));

                // Convert to network byte order (big-endian).
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }

                return bytes;
            }

            return new byte[0];
        }

        public void Set(T data)
        {
            using (MUCOPacket packet = new MUCOPacket((System.UInt16)EPacketIdentifier.MulticastReplicateGenericVariable))
            {
                packet.WriteInt(m_NetworkIdentity.NetworkIdentifier);
                packet.WriteUInt16(m_VariableIdentifier);
                packet.WriteBytes(NetSerializeToByteArray(data));
                ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);
            }

            this.Data = data;
        }

        private static void HandleMulticastReplicateGenericVariable(MUCOPacket packet)
        {
            int networkIdentifier = packet.ReadInt();
        }
    }

}