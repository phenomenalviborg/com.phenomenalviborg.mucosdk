using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    using PacketIdentifier = System.UInt16;

    /*public enum MUCOServerPackets : int // TODO: Replace with System.UInt32
    {
        SpawnUser,
        RemoveUser,
        TranslateUser,
        RotateUser,
        LoadExperience
    }

    public enum MUCOClientPackets : int // TODO: Replace with System.UInt32
    {
        TranslateUser,
        RotateUser,
        DeviceInfo,
    }*/

    public enum EPacketIdentifier : PacketIdentifier
    {
        // MUCOSDK Client packet identiferis 10000-11000
        ClientGenericReplicatedUnicast = 10000,
        ClientGenericReplicatedMulticast = 10001,

        // MUCOSDK Datastore packet identiferis 11000-12000
        DatastoreSet = 11001,

        // MUCOSDK Multicast packet identifers 12000-14000
        MulticastReplicateGenericVariable = 12000,
        MulticastTranslateUser = 12001,
        MulticastRotateUser = 12002,

        // MUCOSDK Unicast packet identifers 14000-16000

        // MUCOSDK Server packet identifiers: 20000-30000
        ServerUserConnected = 20000,
        ServerUserDisconnected = 20001,
        ServerLoadExperience = 20002,
    }
}
