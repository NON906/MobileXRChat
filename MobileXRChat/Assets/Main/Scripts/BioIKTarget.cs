using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    public class BioIKTarget : MonoBehaviour
    {
        public Transform HandBone;
        public Transform HandPosition;
        public Transform Finger;

        void LateUpdate()
        {
            transform.position = HandBone.position - HandPosition.position + Finger.position;
        }
    }
}
