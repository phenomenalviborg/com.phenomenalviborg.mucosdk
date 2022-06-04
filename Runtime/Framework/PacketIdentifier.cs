using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    using PacketIdentifier = System.UInt32;

    public enum EPacketIdentifier : PacketIdentifier
    {
        MulticastTranslateUser = 10000,
        MulticastRotateUser = 10001
    }
}
