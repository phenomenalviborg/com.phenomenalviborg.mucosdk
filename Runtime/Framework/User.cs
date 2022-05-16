using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;

namespace PhenomenalViborg.MUCOSDK
{

    public class User : MonoBehaviour
    {
        public int UserIdentifier = -1;
        public bool IsLocalUser = false;

        public void Initialize(int userIdentifier, bool isLocalUser)
        {
            UserIdentifier = userIdentifier;
            IsLocalUser = isLocalUser;

            if (isLocalUser)
            {
                gameObject.SetActive(false); // To avoid awake on initialization

                // Camera
                gameObject.AddComponent<Camera>();

                // Antilatency tracking
                Antilatency.SDK.AltTrackingUsbSocket altTrackingUsbSocket = gameObject.AddComponent<Antilatency.SDK.AltTrackingUsbSocket>();
                altTrackingUsbSocket.Network = TrackingManager.GetInstance().GetDeviceNetwork();
                altTrackingUsbSocket.Environment = TrackingManager.GetInstance().GetEnvironment();
                
                gameObject.SetActive(true);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Sending packet...");
                MUCOPacket packet = new MUCOPacket((int)MUCOClientPackets.TranslateUser);
                packet.WriteFloat(1.0f);
                packet.WriteFloat(1.0f);
                packet.WriteFloat(1.0f);

                ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);
            }
        }
    }
}
