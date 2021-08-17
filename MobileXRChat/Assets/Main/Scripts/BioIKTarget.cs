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
        public float HandScale;

        void LateUpdate()
        {
            Vector3 pos = (Finger.position - HandPosition.position) * HandScale + HandPosition.position;

            transform.position = HandBone.position - HandPosition.position + pos;
        }
    }
}
