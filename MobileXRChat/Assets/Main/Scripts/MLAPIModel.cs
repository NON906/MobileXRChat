using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Prototyping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HandDVR
{
    public class MLAPIModel : NetworkBehaviour
    {
        public Transform[] BonesTargets = new Transform[(int)HumanBodyBones.LastBone];

        NetworkList<float> morphList_ = new NetworkList<float>(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.OwnerOnly
        });

        Animator animator_;

        IEnumerator Start()
        {
            do
            {
                yield return null;
                animator_ = GetComponentInChildren<Animator>();
            } while (animator_ == null);

            if (IsLocalPlayer)
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    for (int loop = 0; loop < skinnedMeshRenderer.sharedMesh.blendShapeCount; loop++)
                    {
                        morphList_.Add(skinnedMeshRenderer.GetBlendShapeWeight(loop));
                    }
                }
            }
            else
            {
                animator_.enabled = false;
            }
        }

        public override void NetworkStart()
        {
            if (IsLocalPlayer)
            {
                StartCoroutine(sendLoop());
            }
        }

        IEnumerator sendLoop()
        {
            while (animator_ == null)
            {
                yield return null;
            }

            for (; ; )
            {
                yield return new WaitForEndOfFrame();

                for (HumanBodyBones bone = 0; bone < HumanBodyBones.LastBone; bone++)
                {
                    var boneTrans = animator_.GetBoneTransform(bone);
                    if (boneTrans != null)
                    {
                        BonesTargets[(int)bone].position = boneTrans.position;
                        BonesTargets[(int)bone].rotation = boneTrans.rotation;
                    }
                }

                int fullLoop = 0;
                SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    for (int loop = 0; loop < skinnedMeshRenderer.sharedMesh.blendShapeCount; loop++)
                    {
                        float val = skinnedMeshRenderer.GetBlendShapeWeight(loop);
                        if (fullLoop < morphList_.Count && morphList_[fullLoop] != val)
                        {
                            morphList_[fullLoop] = val;
                        }
                        fullLoop++;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (animator_ == null)
            {
                return;
            }

            if (!IsLocalPlayer)
            {
                for (HumanBodyBones bone = 0; bone < HumanBodyBones.LastBone; bone++)
                {
                    var boneTrans = animator_.GetBoneTransform(bone);
                    if (boneTrans != null)
                    {
                        boneTrans.position = BonesTargets[(int)bone].position;
                        boneTrans.rotation = BonesTargets[(int)bone].rotation;
                    }
                }
            }
        }

        void Update()
        {
            if (!IsLocalPlayer)
            {
                int fullLoop = 0;
                SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    for (int loop = 0; loop < skinnedMeshRenderer.sharedMesh.blendShapeCount; loop++)
                    {
                        if (fullLoop < morphList_.Count)
                        {
                            skinnedMeshRenderer.SetBlendShapeWeight(loop, morphList_[fullLoop]);
                            fullLoop++;
                        }
                    }
                }
            }
        }
    }
}
