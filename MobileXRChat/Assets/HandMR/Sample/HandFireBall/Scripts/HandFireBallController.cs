using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace HandMR.HandFireBall
{
    public class HandFireBallController : MonoBehaviour
    {
        [Serializable]
        public class UnityEventVector3Quaternion : UnityEvent<Vector3, Quaternion> { }

        class HandCenterPosition
        {
            public Vector3 Position
            {
                get;
                set;
            }

            public float Time
            {
                get;
                set;
            }
        }

        public UnityEventVector3Quaternion AttackRightPush;
        public UnityEventVector3Quaternion AttackLeftPush;

        public float IgnoreTime = 1f;
        public float MarginTime = 0.5f;
        public float MarginDistance = 0.05f;
        public float IgnoreDistance = 0.4f;
        public float AfterShootIgnoreTime = 0.5f;

        Transform mainCameraTransform_ = null;
        HandVRSphereHand[] sphereHands_;

        List<HandCenterPosition>[] handCenterPosition_ = { new List<HandCenterPosition>(), new List<HandCenterPosition>() };

        bool[] isPushStarted_ = new bool[] { false, false };
        Vector3[] isPushStartPosition_ = new Vector3[2];
        float lastShootTime_ = 0f;

        IEnumerator Start()
        {
            sphereHands_ = FindObjectsOfType<HandVRSphereHand>();

            while (mainCameraTransform_ == null)
            {
                mainCameraTransform_ = sphereHands_[0].GetComponent<SetParentMainCamera>().MainCameraTransform;
                yield return null;
            }
        }

        void Update()
        {
            if (mainCameraTransform_ == null)
            {
                lastShootTime_ = Time.time;
                return;
            }

            if (lastShootTime_ + AfterShootIgnoreTime > Time.time)
            {
                return;
            }

            HandCenterPosition[] centerPositions = { null, null };
            foreach (HandVRSphereHand sphereHand in sphereHands_)
            {
                if (!sphereHand.IsTrackingHand)
                {
                    continue;
                }

                Vector3 handCenter = mainCameraTransform_.InverseTransformPoint(sphereHand.HandCenterPosition);

                if (centerPositions[(int)sphereHand.ThisEitherHand] != null)
                {
                    float oldDistance = (centerPositions[(int)sphereHand.ThisEitherHand].Position - handCenterPosition_[(int)sphereHand.ThisEitherHand][handCenterPosition_.Length - 1].Position).magnitude;
                    float newDistance = (handCenter - handCenterPosition_[(int)sphereHand.ThisEitherHand][handCenterPosition_.Length - 1].Position).magnitude;
                    if (oldDistance < newDistance)
                    {
                        continue;
                    }
                }

                centerPositions[(int)sphereHand.ThisEitherHand] = new HandCenterPosition();
                centerPositions[(int)sphereHand.ThisEitherHand].Position = handCenter;
                centerPositions[(int)sphereHand.ThisEitherHand].Time = Time.time;
            }
            for (int loop = 0; loop < 2; loop++)
            {
                while (handCenterPosition_[loop].Count > 0 && handCenterPosition_[loop][0].Time < Time.time - IgnoreTime)
                {
                    handCenterPosition_[loop].RemoveAt(0);
                }

                while (handCenterPosition_[loop].Count > 1 && handCenterPosition_[loop][1].Time > Time.time - MarginTime)
                {
                    handCenterPosition_[loop].RemoveAt(0);
                }

                if (!isPushStarted_[loop])
                {
                    if (centerPositions[loop] != null)
                    {
                        if (handCenterPosition_[loop].Count > 0)
                        {
                            if (centerPositions[loop].Position.sqrMagnitude > handCenterPosition_[loop][0].Position.sqrMagnitude
                                && (centerPositions[loop].Position - handCenterPosition_[loop][0].Position).magnitude > MarginDistance
                                && (centerPositions[loop].Position - handCenterPosition_[loop][0].Position).magnitude < IgnoreDistance)
                            {
                                isPushStarted_[loop] = true;
                                isPushStartPosition_[loop] = centerPositions[loop].Position;
                            }
                        }
                    }
                }
                else if (handCenterPosition_[loop].Count > 0)
                {
                    if (centerPositions[loop] == null
                        || centerPositions[loop].Position.sqrMagnitude < handCenterPosition_[loop][0].Position.sqrMagnitude
                        || (centerPositions[loop].Position - handCenterPosition_[loop][0].Position).magnitude < MarginDistance)
                    {
                        Vector3 worldPos = mainCameraTransform_.TransformPoint(handCenterPosition_[loop][handCenterPosition_[loop].Count - 1].Position);
                        if (loop == (int)HandVRSphereHand.EitherHand.Right)
                        {
                            AttackRightPush.Invoke(worldPos,
                                Quaternion.LookRotation(worldPos - mainCameraTransform_.TransformPoint(isPushStartPosition_[loop])));
                        }
                        else
                        {
                            AttackLeftPush.Invoke(worldPos,
                                Quaternion.LookRotation(worldPos - mainCameraTransform_.TransformPoint(isPushStartPosition_[loop])));
                        }

                        handCenterPosition_[0].Clear();
                        handCenterPosition_[1].Clear();

                        isPushStarted_[0] = false;
                        isPushStarted_[1] = false;
                        lastShootTime_ = Time.time;
                    }
                }

                if (centerPositions[loop] != null && lastShootTime_ + AfterShootIgnoreTime < Time.time)
                {
                    handCenterPosition_[loop].Add(centerPositions[loop]);
                }
            }
        }
    }
}
