using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public interface IControlObject
    {
        void StartFocus(HandVRSphereHand.EitherHand hand);
        void EndFocus(HandVRSphereHand.EitherHand hand);
        void StartGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
        void StayGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
        void EndGrab(HandVRSphereHand.EitherHand hand);
        void StartTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
        void StayTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
        void EndTouch(HandVRSphereHand.EitherHand hand);
    }
}
