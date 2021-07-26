using DVRSDK.Auth;
using DVRSDK.Avatar;
using DVRSDK.Avatar.Tracking;
using DVRSDK.Serializer;
using DVRSDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace HandDVR
{
    public class LoadAvatar : MonoBehaviour
    {
        public Camera[] FPSCameras;
        public BioIKHandModel BioIKHandModelObject;

        [SerializeField]
        private HandMRTracker handMRTracker = null;

        public Text CurrentStatusText;
        public RendererDisabled[] RendererDisabledObjs;
        public GameObject LocalLipSyncPrefab;

        public bool IsLogined
        {
            private set;
            get;
        } = false;

        private Dictionary<string, VRMLoader> vrmLoaders_ = new Dictionary<string, VRMLoader>();

        private FinalIKCalibrator calibrator_ = null;

        private GameObject currentModel_ = null;
        bool isCalibration_ = false;
        public GameObject CurrentModel
        {
            get
            {
                return isCalibration_ ? currentModel_ : null;
            }
        }

        private Dictionary<ApiRequestErrors, string> apiRequestErrorMessages_ = new Dictionary<ApiRequestErrors, string> {
            { ApiRequestErrors.Unknown,"Unknown request error" },
            { ApiRequestErrors.Forbidden, "Request forbidden" },
            { ApiRequestErrors.Unregistered, "User unregistered" },
            { ApiRequestErrors.Unverified, "User email unverified" },
         };

        bool isStartedLogin_ = false;

        public void DoLogin()
        {
            if (isStartedLogin_)
            {
                return;
            }
            isStartedLogin_ = true;

            var sdkSettings = Resources.Load<SdkSettings>("SdkSettings");
            var client_id = sdkSettings.client_id;
            var config = new DVRAuthConfiguration(client_id, new UnitySettingStore(), new UniWebRequest(), new NewtonsoftJsonSerializer());
            Authentication.Instance.Init(config);

            Authentication.Instance.Authorize(
                openBrowser: url =>
                {
                    Application.OpenURL(url);
                },
                onAuthSuccess: async isSuccess =>
                {
                    if (isSuccess)
                    {
                        CurrentStatusText.text = "Login Success!";
                        IsLogined = true;
                        await loadMyVRM();
                    }
                    else
                    {
                        CurrentStatusText.text = "Login Failed";
                        isStartedLogin_ = false;
                    }
                },
                onAuthError: exception =>
                {
                    CurrentStatusText.text = exception.Message;
                    isStartedLogin_ = false;
                });
        }

        async Task loadMyVRM()
        {
            var currentUser = await Authentication.Instance.Okami.GetCurrentUserAsync();
            currentModel_ = await GetVRM(currentUser.id, true);
            doCalibration();
            vrmLoaders_[currentUser.id].ShowMeshes();
            isCalibration_ = true;
            if (RendererDisabledObjs != null)
            {
                foreach (var disabledObj in RendererDisabledObjs)
                {
                    disabledObj.enabled = true;
                }
            }
        }

        public async Task<GameObject> GetVRM(string id, bool fps = false)
        {
            if (vrmLoaders_.ContainsKey(id))
            {
                vrmLoaders_[id].Dispose();
                vrmLoaders_.Remove(id);
            }

            var vrmLoader = new VRMLoader();
            GameObject currentModel = null;

            try
            {
                var myUser = await Authentication.Instance.Okami.GetUserAsync(id);
                var currentAvatar = myUser.current_avatar;

                currentModel = await Authentication.Instance.Okami.LoadAvatarVRMAsync(currentAvatar, vrmLoader.LoadVRMModelFromConnect) as GameObject;

                if (fps)
                {
                    vrmLoader?.AddAutoBlinkComponent();

                    var vrmFirstPerson = currentModel.GetComponent<VRMFirstPerson>();
                    if (vrmFirstPerson != null) vrmFirstPerson.Setup();
                    foreach (var camera in GameObject.FindObjectsOfType<Camera>())
                    {
                        camera.cullingMask = FPSCameras.ToList().Contains(camera)
                            ? camera.cullingMask & ~(1 << VRMFirstPerson.THIRDPERSON_ONLY_LAYER) //ThirdPerson‚¾‚¯–³Œø
                            : camera.cullingMask & ~(1 << VRMFirstPerson.FIRSTPERSON_ONLY_LAYER) //FirstPerson‚¾‚¯–³Œø
                            ;
                    }

                    var lipsync = Instantiate(LocalLipSyncPrefab);
                    lipsync.GetComponent<DynamicOVRLipSync>().ImportVRMmodel(currentModel);
                }
            }
            catch (ApiRequestException ex)
            {
                Debug.LogError(apiRequestErrorMessages_[ex.ErrorType]);
            }

            if (currentModel != null)
            {
                CurrentStatusText.text = "VRM Loaded";
                vrmLoaders_[id] = vrmLoader;
                return currentModel;
            }
            else
            {
                CurrentStatusText.text = "Download Error";
                return null;
            }
        }

        void doCalibration()
        {
            if (currentModel_ == null) return;
            if (calibrator_ == null) calibrator_ = new FinalIKCalibrator(handMRTracker);
            calibrator_?.LoadModel(currentModel_);
            calibrator_?.DoCalibration();

            BioIKHandModelObject.SetModelAnimator(currentModel_.GetComponentInChildren<Animator>());
        }

        public void ShowMeshes(string id)
        {
            vrmLoaders_[id].ShowMeshes();
        }

        void OnDestroy()
        {
            foreach (var vrmLoader in vrmLoaders_.Values)
            {
                vrmLoader.Dispose();
            }
            vrmLoaders_.Clear();
        }
    }
}
