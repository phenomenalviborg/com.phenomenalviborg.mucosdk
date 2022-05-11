using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public enum MUCOServerPackets : int
    {
        SpawnUser,
        RemoveUser,
        TranslateUser,
        RotateUser,
        LoadExperience
    }

    public enum MUCOClientPackets : int
    {
        TranslateUser,
        RotateUser,
        DeviceInfo
    }
}
