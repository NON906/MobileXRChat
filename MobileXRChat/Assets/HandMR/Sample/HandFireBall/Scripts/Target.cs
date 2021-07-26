using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace HandMR.HandFireBall
{
    public class Target : MonoBehaviour
    {
        public SpawnTarget SpawnTargetObject
        {
            get;
            set;
        } = null;

        public UnityEvent BreakTarget;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<FireBallBullet>() != null)
            {
                BreakTarget.Invoke();
                collision.gameObject.GetComponent<FireBallBullet>().DestroyOnPlay();
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (SpawnTargetObject)
            {
                SpawnTargetObject.RemoveTargets(this);
            }
        }
    }
}
