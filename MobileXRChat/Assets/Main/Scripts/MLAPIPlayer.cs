using DVRSDK.Auth;
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HandDVR
{
    public class MLAPIPlayer : NetworkBehaviour
    {
        NetworkVariableString userId_ = new NetworkVariableString(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.OwnerOnly
        }, null);

        public override async void NetworkStart()
        {
            LoadAvatar loadAvatarObject = FindObjectOfType<LoadAvatar>();

            while (!loadAvatarObject.IsLogined)
            {
                await Task.Delay(1);
            }

            if (IsOwner)
            {
                var currentUser = await Authentication.Instance.Okami.GetCurrentUserAsync();
                userId_.Value = currentUser.id;

                while (loadAvatarObject.CurrentModel == null)
                {
                    await Task.Delay(1);
                }

                loadAvatarObject.CurrentModel.transform.parent = transform;
            }
            else
            {
                while (userId_.Value == null)
                {
                    await Task.Delay(1);
                }

                var model = await loadAvatarObject.GetVRM(userId_.Value);
                model.transform.parent = transform;
                loadAvatarObject.ShowMeshes(userId_.Value);
            }
        }
    }
}
