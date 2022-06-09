using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;

namespace PhenomenalViborg.MUCOSDK
{
    public struct NetworkUser
    {
        public int Identifier;
        public bool IsLocalUser;
    }

    public class ClientNetworkManager : PhenomenalViborg.MUCOSDK.IManager<ClientNetworkManager>, INetEventListener
    {
        private List<NetworkUser> m_NetworkUsers = new List<NetworkUser>();
        public List<NetworkUser> NetworkUsers => m_NetworkUsers;
        
        private NetworkUser m_LocalNetworkUser;

        public delegate void PacketHandler(MUCOPacket packet);
        public Dictionary<System.UInt16, PacketHandler> m_PacketHandlers = new Dictionary<System.UInt16, PacketHandler>();

        private NetManager m_Client;
        private NetDataWriter m_DataWriter;

        public void RegisterPacketHandler(System.UInt16 packetIdentifier, PacketHandler packetHandler)
        {
            if (m_PacketHandlers.ContainsKey(packetIdentifier))
            {
                MUCOLogger.Error($"Failed to register packet handler to packet identifier: {packetIdentifier}. The specified packet identifier has already been assigned a packet handler.");
                return;
            }

            MUCOLogger.Trace($"Successfully assigned a packet handler to packet identifier: {packetIdentifier}");

            m_PacketHandlers.Add(packetIdentifier, packetHandler);
        }


        protected override void Awake()
        {
            base.Awake();

            RegisterPacketHandler((System.UInt16)EPacketIdentifier.ServerUserConnected, HandleUserConnected);
            RegisterPacketHandler((System.UInt16)EPacketIdentifier.ServerUserDisconnected, HandleUserDisconnected);
        }

        public void Connect(string address, int port)
        {
            Debug.Log($"Connecting: {address}:{port}");

            m_Client = new NetManager(this);
            m_DataWriter = new NetDataWriter();
            m_Client.UnconnectedMessagesEnabled = true;
            m_Client.UpdateTime = 15;
            m_Client.Start();
        }

        void Update()
        {
            m_Client.PollEvents();

            var peer = m_Client.FirstPeer;
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                m_Client.SendBroadcast(new byte[] { 1 }, 5000);
            }
        }

        public void AddNetworkUser(NetworkUser networkUser)
        {
            m_NetworkUsers.Add(networkUser);
        }

        public void Disconnect()
        {
            m_Client.Stop();
        }

        private void OnApplicationQuit()
        {
            // TODO: If connected
            Disconnect();
        }

        private static void Log(MUCOLogMessage message)
        {
            MUCOThreadManager.ExecuteOnMainThread(() =>
            {
                Debug.Log(message.ToString());
            });
        }

        public NetworkUser GetLocalNetworkUser()
        {
            return m_LocalNetworkUser;
        }

        private void HandleUserConnected(MUCOPacket packet)
        {
                NetworkUser networkUser;
                networkUser.Identifier = packet.ReadInt();
                networkUser.IsLocalUser = (networkUser.Identifier == m_Client.FirstPeer.RemoteId);

                if (networkUser.IsLocalUser)
                {
                    m_LocalNetworkUser = networkUser;
                }

                Debug.Log($"User Connected: {networkUser.Identifier}");
                m_NetworkUsers.Add(networkUser);
                ExperienceManager.GetInstance().SpawnUser(networkUser);
        }
        private void HandleUserDisconnected(MUCOPacket packet)
        {
                int userIdentifier = packet.ReadInt();
                Debug.Log($"User Disconnected: {userIdentifier}");

                NetworkUser? networkUser = m_NetworkUsers.Find(user => user.Identifier == userIdentifier);
                if (networkUser != null)
                {
                    m_NetworkUsers.Remove((NetworkUser)networkUser);
                    ExperienceManager.GetInstance().RemoveUser((NetworkUser)networkUser);
                }
        }

        public void SendReplicatedUnicastPacket(MUCOPacket packet, NetworkUser receiver)
        {
            using (MUCOPacket unicastPacket = new MUCOPacket((System.UInt16)EPacketIdentifier.ClientGenericReplicatedUnicast))
            {
                packet.SetReadOffset(0);
                unicastPacket.WriteInt((int)receiver.Identifier);
                unicastPacket.WriteBytes(packet.ReadBytes(packet.GetSize()));
                SendPacket(unicastPacket);
            }
        }

        public void SendReplicatedMulticastPacket(MUCOPacket packet)
        {
            using (MUCOPacket multicastPacket = new MUCOPacket((System.UInt16)EPacketIdentifier.ClientGenericReplicatedMulticast))
            {
                packet.SetReadOffset(0);
                multicastPacket.WriteBytes(packet.ReadBytes(packet.GetSize()));
                SendPacket(multicastPacket);
            }
        }

        public void SendPacket(MUCOPacket packet)
        {
            m_DataWriter.Reset();
            m_DataWriter.Put(packet.ToArray(), 0, packet.GetSize());
            //receiver.Send(m_DataWriter, DeliveryMethod.Sequenced);
        }

        void INetEventListener.OnPeerConnected(NetPeer peer) { }
        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) { }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[CLIENT] Error " + socketError);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            MUCOPacket packet = new MUCOPacket((byte[])reader.RawData);

            System.UInt16 packetID = reader.GetUShort();

            if (m_PacketHandlers.ContainsKey(packetID))
            {
                m_PacketHandlers[packetID](packet);
            }
            else
            {
                MUCOLogger.Error($"Failed to find package handler for packet with identifier: {packetID}");
            }
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.BasicMessage && m_Client.ConnectedPeersCount == 0 && reader.GetInt() == 1)
            {
                Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                m_Client.Connect(remoteEndPoint, "sample_app");
            }
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request) { }
        
    }
}