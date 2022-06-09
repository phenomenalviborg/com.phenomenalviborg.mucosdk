using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PhenomenalViborg.MUCONet
{
	/// <summary>
	/// Handles all "low-level" socket communication with the server.
	/// </summary>
	public class MUCOClient
	{
		public delegate void PacketHandler(MUCOPacket packet);

		private Dictionary<UInt16, PacketHandler> m_PacketHandlers = new Dictionary<UInt16, PacketHandler>();

		private byte[] m_ReceiveBuffer = new byte[MUCOConstants.RECEIVE_BUFFER_SIZE];
		private Socket m_LocalSocket;
		private bool m_IsConnected = false;

		public int UniqueIdentifier { get; private set; } = -1;

		public delegate void OnConnectedDelegate();
		public event OnConnectedDelegate OnConnectedEvent;

		/// <summary>
		/// Constructs an instance of MUCOClient.
		/// </summary>
		public MUCOClient()
		{
			RegisterPacketHandler((UInt16)EInternalPacketIdentifier.ServerWelcome, HandleWelcome);
		}

		/// <summary>
		/// Starts the connection process.
		/// </summary>
		/// <param name="address">The IP address to connect to.</param>
		/// <param name="port">The port to connect to.</param>
		public void Connect(string address, int port)
		{
			try
			{
				// The AddressFamily enum specifies the addressing scheme that an instance of the Socket class can use. AddressFamily.InterNetwork represents address for IP version 4 (IPv4).
				// The SocketType enum specifies the type of socket that an instance of the Socket class represents. SocketType.Steam is a reliable, two-way, connection-based byte stream.
				// The ProtocolType enum specifies the protocols that the Socket class supports. ProtocolType.TCP represents Transmission Control Protocol(TCP)
				m_LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				// Configure endpoint
				// TODO: the ip and port should not be const.
				IPAddress ipAddress = IPAddress.Parse(address);
				IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

				MUCOLogger.Info($"Connecting to server {ipEndPoint}");

				// Begins an asynchronous request for a remote host connection.
				m_LocalSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), null);
			}
			catch (Exception exception)
			{
				MUCOLogger.Error($"Failed to create and/or configure the Socket: {exception.Message}");
			}
		}

		/// <summary>
		/// Disconnects from the active connection and resets all runtime data.
		/// </summary>
		public void Disconnect()
        {
			MUCOLogger.Info("Disconnecting...");
			if (m_IsConnected)
            {
				m_IsConnected = false;
				m_LocalSocket.Close();

				MUCOLogger.Info("Disconnected from the server.");
			}
			else
            {
				MUCOLogger.Info("Failed to disconnect, there was no server to disconnect from.");
			}
		}

		/// <summary>
		/// Sends a packet to the server.
		/// </summary>
		/// <param name="packet">The packet to send.</param>
		/// <param name="reliable">Reliable packets are sent using TCP, non-reliable packets use UDP.</param>
		public void SendPacket(MUCOPacket packet, bool reliable = true)
		{
			if (!m_IsConnected) return;

			if (reliable)
			{
				MUCOLogger.Trace($"Sending a packet to the server.");
				packet.WriteLength();
				m_LocalSocket.BeginSend(packet.ToArray(), 0, packet.GetSize(), SocketFlags.None, new AsyncCallback(SendCallback), null);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Registers a packet handler to specifed packet identifier.
		/// </summary>
		/// <param name="packetIdentifier">The packet identifier to assign the packet handler.</param>
		/// <param name="packetHandler">The packet handler delegate.</param>
		public void RegisterPacketHandler(UInt16 packetIdentifier, PacketHandler packetHandler)
		{
			if (m_PacketHandlers.ContainsKey(packetIdentifier))
			{
				MUCOLogger.Error($"Failed to register packet handler to packet identifier: {packetIdentifier}. The specified packet identifier has already been assigned a packet handler.");
				return;
			}

			MUCOLogger.Trace($"Successfully assigned a packet handler to packet identifier: {packetIdentifier}");

			m_PacketHandlers.Add(packetIdentifier, packetHandler);
		}

		/// <summary>
		/// An asynchronous callback used for handling an incoming connection from the remote host.
		/// </summary>
		private void ConnectCallback(IAsyncResult asyncResult)
		{
			try
			{
				m_LocalSocket.EndConnect(asyncResult);

				MUCOLogger.Info("Connection was successfully established with the server.");
				m_IsConnected = true;

				m_LocalSocket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
			}
			catch (Exception exception)
			{
				MUCOLogger.Error($"Failed to accept request from remote host: {exception.Message}");
			}
		}

		/// <summary>
		/// An asynchronous callback used when sending data.
		/// </summary>
		private void SendCallback(IAsyncResult asyncResult)
		{
			try
			{
				m_LocalSocket.EndSend(asyncResult);
			}
			catch (Exception exception)
			{
				MUCOLogger.Error($"An error occurred when sending data: {exception.Message}");
			}
		}

		/// <summary>
		/// An asynchronous callback used for handling incoming data.
		/// </summary>
		int i = 0;
		private MUCOPacket receivedData = new MUCOPacket();
		private void RecieveCallback(IAsyncResult asyncResult)
		{
			MUCOLogger.Error(i.ToString());
			i++;

			try
			{
				int bytesReceived = m_LocalSocket.EndReceive(asyncResult);
				MUCOLogger.Trace($"Receiving package from server.");
				if (bytesReceived <= 0)
				{
					Disconnect();
					return;
				}

				byte[] dataReceived = new byte[bytesReceived];
				Array.Copy(m_ReceiveBuffer, dataReceived, bytesReceived);

				if (HandleData(dataReceived))
				{
					receivedData.Reset();
				}

				// Begin an asynchronously operation to receive incoming data from clientSocket. Incoming data will be stored in m_ReceiveBuffer 
				m_LocalSocket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
			}
			catch (SocketException exception)
			{
				if (exception.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset)
				{
					Disconnect();
				}
				else
				{
					MUCOLogger.Error($"A socket exception occurred while receiving data: {exception.Message}");
				}
			}
			catch (Exception exception)
			{
				MUCOLogger.Error($"An error occurred while receiving data: {exception.Message}");
			}
		}

		/// <summary>
		/// Makes sure that the specified packet gets passed on the correct PacketHandler.
		/// </summary>
		/// <param name="packet">The packet to handle</param>
		private bool HandleData(byte[] _data)
		{
			MUCOLogger.Info("HandleData()");

			int _packetLength = 0;

			receivedData.WriteBytes(_data);

			if (receivedData.UnreadLength() >= 4)
			{
				// If client's received data contains a packet
				_packetLength = receivedData.ReadInt() - 4;
				if (_packetLength <= 0)
				{
					// If packet contains no data
					return true; // Reset receivedData instance to allow it to be reused
				}
			}

			while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
			{
				MUCOLogger.Info("Test");

				// While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
				byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
				PhenomenalViborg.MUCOSDK.MUCOThreadManager.ExecuteOnMainThread(() =>
				{
					using (MUCOPacket _packet = new MUCOPacket(_packetBytes))
					{

						System.UInt16 _packetId = _packet.ReadUInt16();
						MUCOLogger.Info($"PacketID: {_packetId}");
						if (m_PacketHandlers.ContainsKey(_packetId))
							m_PacketHandlers[_packetId](_packet); // Call appropriate method to handle the packet
					}
				});

				_packetLength = 0; // Reset packet length
				if (receivedData.UnreadLength() >= 4)
				{
					// If client's received data contains another packet
					_packetLength = receivedData.ReadInt();
					if (_packetLength <= 0)
					{
						// If packet contains no data
						return true; // Reset receivedData instance to allow it to be reused
					}
				}

			}

			if (_packetLength <= 1)
			{
				return true; // Reset receivedData instance to allow it to be reused
			}

			return false;


			/*int size = packet.ReadInt();
			UInt16 packetID = packet.ReadUInt16();

			if (m_PacketHandlers.ContainsKey(packetID))
			{
				m_PacketHandlers[packetID](packet);
			}
			else
			{
				MUCOLogger.Error($"Failed to find package handler for packet with identifier: {packetID}");
			}*/
		}

		#region Internal package handlers
		private void HandleWelcome(MUCOPacket packet)
		{
			int assignedClientID = packet.ReadInt();
			UniqueIdentifier = assignedClientID;
			MUCOLogger.Info($"Welcome, {assignedClientID}");

			OnConnectedEvent?.Invoke();

			MUCOPacket welcomeRecivedPacket = new MUCOPacket((UInt16)EInternalPacketIdentifier.ClientWelcomeRecived);
			welcomeRecivedPacket.WriteInt(assignedClientID);
			SendPacket(welcomeRecivedPacket);
		}
		#endregion
	}
}