using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace HandDVR
{
    public class PermissionRequest : MonoBehaviour
    {
        void Start()
        {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        }
    }
}
