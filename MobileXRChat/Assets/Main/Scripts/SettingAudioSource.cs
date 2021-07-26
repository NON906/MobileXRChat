using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    [RequireComponent(typeof(AudioSource))]
    public class SettingAudioSource : MonoBehaviour
    {
        void Update()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.spatialize = true;
            audioSource.spatializePostEffects = true;
            audioSource.spatialBlend = 1f;
        }
    }
}
