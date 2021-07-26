using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HandMR.HandFireBall
{
    public class SpawnTarget : MonoBehaviour
    {
        public GameObject TargetPrefab;

        public float SpawnInterval = 3f;
        public float SpawnEreaSize = 2f;

        List<Target> targets_ = new List<Target>();
        public Target[] Targets
        {
            get
            {
                return targets_.ToArray();
            }
        }

        public UnityAction AddScore
        {
            get;
            set;
        } = null;

        float lastSpawnTime_ = 0f;

        void Update()
        {
            while (Time.time - lastSpawnTime_ > SpawnInterval)
            {
                Vector3 pos = transform.position;
                pos.x = UnityEngine.Random.Range(-SpawnEreaSize, SpawnEreaSize);
                pos.y = UnityEngine.Random.Range(0f, SpawnEreaSize);
                GameObject newTarget = Instantiate(TargetPrefab, pos, Quaternion.identity);
                targets_.Add(newTarget.GetComponent<Target>());
                Target newTargetScript = newTarget.GetComponent<Target>();
                newTargetScript.SpawnTargetObject = this;
                if (AddScore != null)
                {
                    newTargetScript.BreakTarget.AddListener(AddScore);
                }

                if (lastSpawnTime_ == 0f)
                {
                    lastSpawnTime_ = Time.time;
                }
                else
                {
                    lastSpawnTime_ += SpawnInterval;
                }
            }
        }

        public void RemoveTargets(Target target)
        {
            targets_.Remove(target);
        }

        void OnDestroy()
        {
            foreach (Target target in targets_)
            {
                Destroy(target.gameObject);
            }
        }
    }
}
