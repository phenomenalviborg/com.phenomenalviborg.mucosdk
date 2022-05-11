using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{

    public class User : MonoBehaviour
    {
        public int UserIdentifier /*{ get; private set; }*/ = -1;
        public bool IsLocalUser = false;

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;

            InvokeRepeating("SendDeviceInfo", 0.0f, 1.0f);

            if (isLocalUser)
            {
                gameObject.AddComponent<Camera>();

                gameObject.SetActive(false); // To avoid awake on initialization
                Antilatency.SDK.AltTrackingUsbSocket altTrackingUsbSocket = gameObject.AddComponent<Antilatency.SDK.AltTrackingUsbSocket>();
                altTrackingUsbSocket.Network = TrackingManager.GetInstance().GetDeviceNetwork();
                altTrackingUsbSocket.Environment = TrackingManager.GetInstance().GetEnvironment();
                gameObject.SetActive(true);
            }

        }

    }
}
