using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOUser : MonoBehaviour
    {
        // TODO: Custom data type
        [HideInInspector] public int m_UserIdentifier { get; private set; } = 0;
    }
}