using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR.HandFireBall
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(ParticleSystem))]
    public class FireBallBullet : MonoBehaviour
    {
        public float Speed = 10f;
        public float HomingAngleRate = 0.1f;

        public SpawnTarget SpawnTargetObject
        {
            get;
            set;
        } = null;

        Rigidbody rigidbody_;
        Target nearTarget_ = null;

        void Start()
        {
            rigidbody_ = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            Target[] targets = null;
            if (SpawnTargetObject != null)
            {
                targets = SpawnTargetObject.Targets;
            }
            else
            {
                targets = FindObjectsOfType<Target>();
            }
            if (targets != null && targets.Length > 0)
            {
                if (nearTarget_ == null)
                {
                    float nearDistance = float.PositiveInfinity;
                    foreach (Target target in targets)
                    {
                        float distance = Mathf.Abs(Mathf.Acos(Vector3.Dot((target.transform.position - transform.position).normalized, transform.forward)));
                        if (nearDistance > distance)
                        {
                            nearDistance = distance;
                            nearTarget_ = target;
                        }
                    }
                }

                Vector3 targetDirection = transform.InverseTransformPoint(nearTarget_.transform.position);
                targetDirection.z = 0f;
                targetDirection.Normalize();
                float tmp = targetDirection.x;
                targetDirection.x = -targetDirection.y;
                targetDirection.y = tmp;
                rigidbody_.AddTorque(targetDirection * HomingAngleRate - rigidbody_.angularVelocity, ForceMode.VelocityChange);
            }

            rigidbody_.AddForce(transform.forward * Speed - rigidbody_.velocity, ForceMode.VelocityChange);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "BulletDestroy")
            {
                DestroyOnPlay();
            }
        }

        public void DestroyOnPlay()
        {
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            rigidbody_.isKinematic = true;

            Renderer[] renderers = GetComponents<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            StartCoroutine(destroyCoroutine());
        }

        IEnumerator destroyCoroutine()
        {
            ParticleSystem particleSystem = GetComponent<ParticleSystem>();
            var particleMain = particleSystem.main;
            particleMain.loop = false;
            particleMain.stopAction = ParticleSystemStopAction.Disable;

            yield return new WaitForSeconds(particleMain.duration);

            Destroy(gameObject);
        }
    }
}
