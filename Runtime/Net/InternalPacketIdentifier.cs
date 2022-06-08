using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhenomenalViborg.MUCONet
{
    /*
    using PacketIdentifier = System.UInt16;

    public struct PacketIdentifier
    {
        private System.UInt16 m_Identifier;

        public static implicit operator PacketIdentifier(System.UInt16 identifier)
        {
            return new PacketIdentifier { m_Identifier = identifier };
        }

        public static implicit operator System.UInt16(PacketIdentifier packetIdentifier)
        {
            return packetIdentifier.m_Identifier;
        }
    }
    
    Trash language, no way of doing global type aliases; this will change when ported to c++...
    */

    // 30000-40000: Internal server packets
    // 40000-50000: Internal client packets
    enum EInternalPacketIdentifier : System.UInt16
    {
        ServerWelcome = 30000,
        ClientWelcomeRecived = 40000,
    }
}
