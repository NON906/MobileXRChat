using DVRSDK.Avatar.Tracking;
using HandMR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HandDVR
{
    public class HandMRTracker : MonoBehaviour, ITracker
    {
        [SerializeField]
        private Transform trackersParent = null;
        public Transform TrackersParent
        {
            get
            {
                return trackersParent;
            }
            set
            {
                trackersParent = value;
                var cameraTarget = FindObjectOfType<HandMRManager>().GetMainObject().GetComponentInChildren<CameraTarget>();
                cameraTarget.CenterTransform = trackersParent;
            }
        }

        public Transform HeadTransform;
        public Transform LeftHandTransform;
        public Transform RightHandTransform;

        public readonly TrackerTarget[] TrackerTargets = new TrackerTarget[]
        {
            new TrackerTarget { TrackerPosition = TrackerPositions.Head, UseDeviceType = TrackingDeviceType.HMD },
            new TrackerTarget { TrackerPosition = TrackerPositions.LeftHand },
            new TrackerTarget { TrackerPosition = TrackerPositions.RightHand },
        };

        public TrackerTarget GetTrackerTarget(TrackerPositions trackerPosition) => TrackerTargets.FirstOrDefault(d => d.TrackerPosition == trackerPosition && d.PoseIsValid);

        public Vector3 GetIKOffsetPosition(TrackerPositions targetPosition, TrackingDeviceType deviceType)
        {
            return Vector3.zero;
        }
        public Quaternion GetIKOffsetRotation(TrackerPositions targetPosition, TrackingDeviceType deviceType)
        {
            /*
            if (targetPosition == TrackerPositions.LeftHand)
            {
                return Quaternion.Euler(0, 90, 270);
            }
            else if (targetPosition == TrackerPositions.RightHand)
            {
                return Quaternion.Euler(0, -90, -270);
            }
            else
            {
                return Quaternion.identity;
            }
            */
            return Quaternion.identity;
        }

        void Awake()
        {
            TrackerTargets[0].TargetTransform = HeadTransform;
            TrackerTargets[1].TargetTransform = LeftHandTransform;
            TrackerTargets[2].TargetTransform = RightHandTransform;

            TrackerTargets[0].PoseIsValid = true;
            TrackerTargets[1].PoseIsValid = true;
            TrackerTargets[2].PoseIsValid = true;
        }

        void Start()
        {
            var cameraTarget = FindObjectOfType<HandMRManager>().GetMainObject().GetComponentInChildren<CameraTarget>();
            cameraTarget.CenterTransform = trackersParent;
        }
    }
}
