using Dissonance;
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    public class MLAPIDissonancePlayer : NetworkBehaviour, IDissonancePlayer
    {
        NetworkVariableString playerId_ = new NetworkVariableString(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.OwnerOnly
        }, "");

        public string PlayerId
        {
            get
            {
                return playerId_.Value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return transform.rotation;
            }
        }

        public NetworkPlayerType Type
        {
            get
            {
                if (NetworkObject == null)
                {
                    return NetworkPlayerType.Unknown;
                }

                if (NetworkManager.Singleton.LocalClientId == NetworkObject.OwnerClientId)
                {
                    return NetworkPlayerType.Local;
                }
                else
                {
                    return NetworkPlayerType.Remote;
                }
            }
        }

        public bool IsTracking
        {
            get;
            private set;
        } = false;

        public override void NetworkStart()
        {
            StartCoroutine(networkStartCoroutine());
        }

        IEnumerator networkStartCoroutine()
        {
            var comms = FindObjectOfType<DissonanceComms>();

            if (Type == NetworkPlayerType.Local)
            {
                setPlayerName(comms.LocalPlayerName);
                comms.LocalPlayerNameChanged += setPlayerName;
            }
            else
            {
                while (string.IsNullOrEmpty(playerId_.Value))
                {
                    yield return null;
                }
            }

            comms.TrackPlayerPosition(this);
            IsTracking = true;
        }

        void setPlayerName(string playerName)
        {
            playerId_.Value = playerName;
        }
    }
}
