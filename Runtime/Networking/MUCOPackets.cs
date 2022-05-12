using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public enum MUCOServerPackets : int // TODO: Replace with System.UInt32
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
        ReplicatedMulticast,
        ReplicatedUnicast
    }
}
