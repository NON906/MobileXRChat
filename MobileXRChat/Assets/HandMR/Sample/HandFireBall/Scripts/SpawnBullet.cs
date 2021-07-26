using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR.HandFireBall
{
    public class SpawnBullet : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public float CoolDownTime = 0.5f;
        public SpawnTarget SpawnTargetObject;

        float lastSpawnTime_ = 0f;

        static List<GameObject> bullets_ = new List<GameObject>();

        void Start()
        {
            SpawnTargetObject = FindObjectOfType<SpawnTarget>();
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {
            if (lastSpawnTime_ + CoolDownTime < Time.time)
            {
                GameObject newObject = Instantiate(BulletPrefab, position, rotation);
                newObject.GetComponent<FireBallBullet>().SpawnTargetObject = SpawnTargetObject;
                bullets_.Add(newObject);

                lastSpawnTime_ = Time.time;
            }
        }

        public static void ClearBullets()
        {
            foreach (GameObject bullet in bullets_)
            {
                if (bullet != null)
                {
                    Destroy(bullet);
                }
            }
            bullets_.Clear();
        }
    }
}
