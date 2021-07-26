using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    [RequireComponent(typeof(NetworkManager))]
    public class MLAPIStart : MonoBehaviour
    {
        bool isStart_ = false;

#if UNITY_STANDALONE
        void Start()
        {
            var networkManager = GetComponent<NetworkManager>();
            networkManager.StartServer();
        }
#endif

        public void StartClient()
        {
            if (isStart_)
            {
                return;
            }

            var networkManager = GetComponent<NetworkManager>();
            networkManager.StartClient();
            isStart_ = true;
        }
    }
}