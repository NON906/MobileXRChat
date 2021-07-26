using BioIK;
using HandMR;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    public class BioIKHandModel : MonoBehaviour
    {
        const float FINGER_LENGTH = 0.015f;

        public float WeightDownSpeed = 1f;
        public RuntimeAnimatorController DefaultAnimatorController = null;

        Animator modelAnimator_ = null;

        VRIK vrik_;
        BioIK.BioIK bioIK_ = null;
        List<Behaviour> leftBioBehaviours_ = new List<Behaviour>();
        List<Behaviour> rightBioBehaviours_ = new List<Behaviour>();

        HandVRSphereHand leftHand_;
        HandVRSphereHand rightHand_;

        float leftPositionWeight_ = 0f;
        float rightPositionWeight_ = 0f;

        public void SetModelAnimator(Animator animator)
        {
            modelAnimator_ = animator;
            addAndSettingIKComponents();
        }

        void addAndSettingIKComponents()
        {
            if (modelAnimator_ == null || !enabled)
            {
                return;
            }

            if (modelAnimator_.runtimeAnimatorController == null && DefaultAnimatorController != null)
            {
                modelAnimator_.runtimeAnimatorController = DefaultAnimatorController;
            }

            Animator animator = modelAnimator_;

            var hands = FindObjectsOfType<HandVRSphereHand>();
            foreach (var hand in hands)
            {
                if (hand.ThisEitherHand == HandVRSphereHand.EitherHand.Left)
                {
                    leftHand_ = hand;
                }
                else if (hand.ThisEitherHand == HandVRSphereHand.EitherHand.Right)
                {
                    rightHand_ = hand;
                }
            }

            void addFingerIKTargets(HumanBodyBones bone, float fingerLength)
            {
                Transform trans = animator.GetBoneTransform(bone);

                if (trans != null && trans.childCount <= 0)
                {
                    GameObject newObj = new GameObject("FingerTarget");
                    newObj.transform.parent = trans;
                    newObj.transform.position = (trans.position - trans.parent.position).normalized * fingerLength + trans.position;
                    newObj.transform.rotation = trans.rotation;
                }
            }
            addFingerIKTargets(HumanBodyBones.LeftThumbIntermediate, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.LeftIndexDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.LeftMiddleDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.LeftRingDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.LeftLittleDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.RightThumbIntermediate, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.RightIndexDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.RightMiddleDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.RightRingDistal, FINGER_LENGTH);
            addFingerIKTargets(HumanBodyBones.RightLittleDistal, FINGER_LENGTH);

            if (bioIK_ == null)
            {
                bioIK_ = animator.GetBoneTransform(HumanBodyBones.Hips).gameObject.AddComponent<BioIK.BioIK>();
            }
            bioIK_.SetGenerations(2);
            bioIK_.SetPopulationSize(75);
            bioIK_.SetElites(2);
            bioIK_.Smoothing = 0f;

            BioSegment bioSegment;
            BioJoint bioJoint;
            Position bioPosition;
            Transform boneTrans;

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = true;
            bioJoint.X.Constrained = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = false;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightHand);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = true;
            bioJoint.X.Constrained = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = false;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -15f;
            bioJoint.Y.UpperLimit = 15f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -15f;
            bioJoint.Y.UpperLimit = 15f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                target.HandPosition = leftHand_.GetFinger(0);
                target.Finger = leftHand_.GetFinger(4);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            leftBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                target.HandPosition = rightHand_.GetFinger(0);
                target.Finger = rightHand_.GetFinger(4);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            rightBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 25f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                bioPosition.SetTargetTransform(leftHand_.GetFinger(5));
            }
            leftBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 25f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                bioPosition.SetTargetTransform(rightHand_.GetFinger(5));
            }
            rightBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                target.HandPosition = leftHand_.GetFinger(0);
                target.Finger = leftHand_.GetFinger(8);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            leftBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                target.HandPosition = rightHand_.GetFinger(0);
                target.Finger = rightHand_.GetFinger(8);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            rightBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                bioPosition.SetTargetTransform(leftHand_.GetFinger(9));
            }
            leftBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                bioPosition.SetTargetTransform(rightHand_.GetFinger(9));
            }
            rightBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                target.HandPosition = leftHand_.GetFinger(0);
                target.Finger = leftHand_.GetFinger(12);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            leftBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                target.HandPosition = rightHand_.GetFinger(0);
                target.Finger = rightHand_.GetFinger(12);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            rightBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                bioPosition.SetTargetTransform(leftHand_.GetFinger(13));
            }
            leftBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -5f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                bioPosition.SetTargetTransform(rightHand_.GetFinger(13));
            }
            rightBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftRingDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                target.HandPosition = leftHand_.GetFinger(0);
                target.Finger = leftHand_.GetFinger(16);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            leftBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightRingDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                target.HandPosition = rightHand_.GetFinger(0);
                target.Finger = rightHand_.GetFinger(16);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            rightBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -20f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                bioPosition.SetTargetTransform(leftHand_.GetFinger(17));
            }
            leftBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = true;
            bioJoint.Y.Constrained = true;
            bioJoint.Y.LowerLimit = -20f;
            bioJoint.Y.UpperLimit = 5f;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = -15f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            /*
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                bioPosition.SetTargetTransform(rightHand_.GetFinger(17));
            }
            rightBioBehaviours_.Add(bioPosition);
            */

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
            bioJoint = bioIK_.FindSegment(boneTrans).AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            leftBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (leftHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                target.HandPosition = leftHand_.GetFinger(0);
                target.Finger = leftHand_.GetFinger(20);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            leftBioBehaviours_.Add(bioPosition);

            boneTrans = animator.GetBoneTransform(HumanBodyBones.RightLittleDistal);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioJoint = bioSegment.AddJoint();
            bioJoint.X.Enabled = false;
            bioJoint.Y.Enabled = false;
            bioJoint.Z.Enabled = true;
            bioJoint.Z.Constrained = true;
            bioJoint.Z.LowerLimit = 0f;
            bioJoint.Z.UpperLimit = 30f;
            bioJoint.JointType = JointType.Rotational;
            bioJoint.SetOrientation(Vector3.zero);
            rightBioBehaviours_.Add(bioJoint);
            boneTrans = boneTrans.GetChild(0);
            bioSegment = bioIK_.FindSegment(boneTrans);
            bioPosition = bioSegment.AddObjective(ObjectiveType.Position) as Position;
            if (rightHand_ != null)
            {
                var targetObj = new GameObject();
                var target = targetObj.AddComponent<BioIKTarget>();
                target.HandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                target.HandPosition = rightHand_.GetFinger(0);
                target.Finger = rightHand_.GetFinger(20);
                bioPosition.SetTargetTransform(targetObj.transform);
            }
            rightBioBehaviours_.Add(bioPosition);

            vrik_ = animator.GetComponentInChildren<VRIK>();
        }

        void FixedUpdate()
        {
            if (modelAnimator_ == null)
            {
                return;
            }

            if (leftHand_ != null && leftHand_.IsTrackingHand)
            {
                leftPositionWeight_ = 1f;
            }
            else
            {
                leftPositionWeight_ -= WeightDownSpeed * Time.fixedDeltaTime;
                if (leftPositionWeight_ < 0f)
                {
                    leftPositionWeight_ = 0f;
                }
            }
            vrik_.solver.leftArm.positionWeight = leftPositionWeight_;
            vrik_.solver.leftArm.rotationWeight = leftPositionWeight_;
            foreach (Behaviour behavior in leftBioBehaviours_)
            {
                if (behavior is Position)
                {
                    ((Position)behavior).SetWeight(leftPositionWeight_);
                    if (leftPositionWeight_ <= 0f)
                    {
                        behavior.enabled = false;
                    }
                    else if (leftPositionWeight_ >= 1f)
                    {
                        behavior.enabled = true;
                    }
                }
                else
                {
                    if (leftPositionWeight_ < 1f)
                    {
                        behavior.enabled = false;
                    }
                    else
                    {
                        behavior.enabled = true;
                    }
                }
            }

            if (rightHand_ != null && rightHand_.IsTrackingHand)
            {
                rightPositionWeight_ = 1f;
            }
            else
            {
                rightPositionWeight_ -= WeightDownSpeed * Time.fixedDeltaTime;
                if (rightPositionWeight_ < 0f)
                {
                    rightPositionWeight_ = 0f;
                }
            }
            vrik_.solver.rightArm.positionWeight = rightPositionWeight_;
            vrik_.solver.rightArm.rotationWeight = rightPositionWeight_;
            foreach (Behaviour behavior in rightBioBehaviours_)
            {
                if (behavior is Position)
                {
                    ((Position)behavior).SetWeight(rightPositionWeight_);
                    if (rightPositionWeight_ <= 0f)
                    {
                        behavior.enabled = false;
                    }
                    else if (rightPositionWeight_ >= 1f)
                    {
                        behavior.enabled = true;
                    }
                }
                else
                {
                    if (rightPositionWeight_ < 1f)
                    {
                        behavior.enabled = false;
                    }
                    else
                    {
                        behavior.enabled = true;
                    }
                }
            }
        }
    }
}
