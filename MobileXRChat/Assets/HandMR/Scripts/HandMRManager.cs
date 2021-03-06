using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Hologla;

namespace HandMR
{
    public class HandMRManager : MonoBehaviour
    {
        public enum Mode
        {
            MR,
            VR,
            AR,
            VRSingle
        };

        public enum HandDetection
        {
            Both = 0,
            LeftOnly = HandVRSphereHand.EitherHand.Left + 1,
            RightOnly = HandVRSphereHand.EitherHand.Right + 1,
            None = 3,
        };

        [SerializeField]
        Mode ViewMode;
        public Mode CurrentViewMode
        {
            get
            {
                return ViewMode;
            }
        }

        public GameObject[] VRBackgroundObjects;
        public bool VRSkyBox = false;
        public Color VRBackColor;
        public bool FreezePositionX = false;
        public bool FreezePositionY = false;
        public bool FreezePositionZ = false;
        public HandDetection HandDetectionMode = HandDetection.Both;
        public bool SkipDetectFloor = false;

        public SetParentMainCamera[] Hands;
        public GameObject MRObject;
        public Transform MRCamera;
        public GameObject ARObject;
        public Transform ARCamera;
        public Camera BackGroundCamera;
        public GameObject VRSubCamera;
        public GameObject LeftEyeFrame;
        public GameObject RightEyeFrame;

        Fisheye[] fisheyes_;
        public float FieldOfView
        {
            get;
            set;
        }

        void Awake()
        {
            if (SkipDetectFloor)
            {
                var quad = MRObject.GetComponentInChildren<ResizeBackGroundQuad>();
                quad.GetComponent<MeshRenderer>().enabled = false;
                quad.transform.parent.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
                MRObject.GetComponentInChildren<CameraTarget>().PlaneDetectEnabled = false;
                ARObject.GetComponentInChildren<CameraTarget>().PlaneDetectEnabled = false;
            }
        }

        void Start()
        {
            fisheyes_ = MRObject.GetComponentsInChildren<Fisheye>();

            viewModeChange();
        }

        void viewModeChange()
        {
            switch (ViewMode)
            {
                case Mode.MR:
                    {
                        MRObject.SetActive(true);
                        ARObject.SetActive(false);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = MRCamera;
                        }
                        foreach (GameObject obj in VRBackgroundObjects)
                        {
                            if (obj != null)
                            {
                                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer.enabled = false;
                                }
                            }
                        }
                        CameraTarget cameraTarget = MRObject.GetComponentInChildren<CameraTarget>();
                        cameraTarget.BackgroundObj.SetActive(true);
                        cameraTarget.PlaneDetectEnabled = !SkipDetectFloor;
                        VRSubCamera.SetActive(false);

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = MRCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.VR:
                    {
                        MRObject.SetActive(true);
                        MRObject.GetComponentInChildren<HologlaCameraManager>().SwitchViewMode(HologlaCameraManager.ViewMode.VR);
                        ARObject.SetActive(false);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = MRCamera;
                        }
                        foreach (GameObject obj in VRBackgroundObjects)
                        {
                            if (obj != null)
                            {
                                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer.enabled = true;
                                }
                            }
                        }
                        CameraTarget cameraTarget = MRObject.GetComponentInChildren<CameraTarget>();
                        cameraTarget.BackgroundObjAutoDisable = true;
                        cameraTarget.PlaneDetectEnabled = !SkipDetectFloor;
                        cameraTarget.BackgroundObj.SetActive(false);
                        VRSubCamera.SetActive(true);
                        cameraTarget.BackgroundObj = VRSubCamera;
                        ResizeBackGroundQuad resizeBackGroundQuad = MRObject.GetComponentInChildren<ResizeBackGroundQuad>();
                        resizeBackGroundQuad.NoticeTextCenter = false;
                        Fisheye[] fisheyes = MRObject.GetComponentsInChildren<Fisheye>();
                        foreach (Fisheye fisheye in fisheyes)
                        {
                            fisheye.enabled = true;
                            Camera camera = fisheye.GetComponent<Camera>();
                            camera.fieldOfView = 90f;
                        }
                        LeftEyeFrame.SetActive(false);
                        RightEyeFrame.SetActive(false);

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = MRCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.VRSingle:
                    {
                        MRObject.SetActive(false);
                        ARObject.SetActive(true);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = ARCamera;
                        }
                        foreach (GameObject obj in VRBackgroundObjects)
                        {
                            if (obj != null)
                            {
                                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer.enabled = false;
                                }
                            }
                        }
                        ARObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = true;
                        ARObject.GetComponentInChildren<CameraTarget>().PlaneDetectEnabled = !SkipDetectFloor;
                        ARObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = true;
                        StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.SolidColor));

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = ARCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.AR:
                    {
                        MRObject.SetActive(false);
                        ARObject.SetActive(true);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = ARCamera;
                        }
                        foreach (GameObject obj in VRBackgroundObjects)
                        {
                            if (obj != null)
                            {
                                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                                {
                                    renderer.enabled = false;
                                }
                            }
                        }
                        ARObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = false;
                        ARObject.GetComponentInChildren<CameraTarget>().PlaneDetectEnabled = !SkipDetectFloor;
                        ARObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = true;
                        StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.Depth));

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = ARCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
            }
        }

        IEnumerator vrCameraSettingCoroutine(CameraClearFlags cameraClearFlags)
        {
            yield return null;

            arCameraSetting(cameraClearFlags);
        }

        void arCameraSetting(CameraClearFlags cameraClearFlags)
        {
            Camera cam = ARCamera.GetComponent<Camera>();
            var background = cam.backgroundColor;
            var cullingMask = cam.cullingMask;
            var depth = cam.depth;
            var targetEye = cam.stereoTargetEye;
            cam.CopyFrom(BackGroundCamera);
            cam.clearFlags = cameraClearFlags;
            cam.targetTexture = null;
            cam.backgroundColor = background;
            cam.cullingMask = cullingMask;
            cam.depth = depth;
            cam.stereoTargetEye = targetEye;

            float degRad = cam.fieldOfView * Mathf.Deg2Rad;
            float newDegRad = Mathf.Atan(Mathf.Tan(degRad * 0.5f) * 0.5f) * 2f;
            float newDeg = newDegRad * Mathf.Rad2Deg;
            cam.fieldOfView = newDeg;
        }

        public void ViewModeChange(Mode mode)
        {
            ViewMode = mode;
            viewModeChange();
        }

        void Update()
        {
            Hands[0].gameObject.SetActive(HandDetectionMode == HandDetection.Both ||
                HandDetectionMode == (HandDetection)(Hands[0].GetComponent<HandVRSphereHand>().ThisEitherHand + 1));
            Hands[1].gameObject.SetActive(HandDetectionMode == HandDetection.Both ||
                HandDetectionMode == (HandDetection)(Hands[1].GetComponent<HandVRSphereHand>().ThisEitherHand + 1));

            if (HandDetectionMode == HandDetection.None)
            {
                if (!Hands[0].gameObject.activeSelf)
                {
                    FindObjectOfType<HandVRMain>().gameObject.SetActive(false);
                }
            }
            else
            {
                FindObjectOfType<HandVRMain>().BothHand = HandDetectionMode == HandDetection.Both;
            }
        }

        void LateUpdate()
        {
            if (ViewMode == Mode.VR)
            {
                foreach (Fisheye fisheye in fisheyes_)
                {
                    Camera camera = fisheye.GetComponent<Camera>();
                    if (!VRSubCamera.activeInHierarchy)
                    {
                        if (VRSkyBox)
                        {
                            camera.clearFlags = CameraClearFlags.Skybox;
                        }
                        else
                        {
                            camera.clearFlags = CameraClearFlags.SolidColor;
                            camera.backgroundColor = VRBackColor;
                        }
                    }
                    else
                    {
                        camera.clearFlags = CameraClearFlags.SolidColor;
                        camera.backgroundColor = Color.black;
                    }

                    camera.fieldOfView = FieldOfView;
                }
            }
        }

        public Transform GetCameraTransform()
        {
            Transform ret = null;

            switch (ViewMode)
            {
                case Mode.MR:
                case Mode.VR:
                    ret = MRCamera;
                    break;
                case Mode.VRSingle:
                case Mode.AR:
                    ret = ARCamera;
                    break;
            }

            return ret;
        }

        public GameObject GetMainObject()
        {
            GameObject ret = null;

            switch (ViewMode)
            {
                case Mode.MR:
                case Mode.VR:
                    ret = MRObject;
                    break;
                case Mode.VRSingle:
                case Mode.AR:
                    ret = ARObject;
                    break;
            }

            return ret;
        }
    }
}
