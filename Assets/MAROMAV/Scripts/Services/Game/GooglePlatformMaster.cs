using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MAROMAV.CoworkSpace
{
    public class GooglePlatformMaster : MonoBehaviour
    {
        // Java class, method, and field constants.
        private readonly int ANDROID_MIN_DAYDREAM_API = 24;
        private readonly string FIELD_SDK_INT = "SDK_INT";
        private readonly string PACKAGE_BUILD_VERSION = "android.os.Build$VERSION";
        private readonly string PACKAGE_DAYDREAM_API_CLASS = "com.google.vr.ndk.base.DaydreamApi";
        private readonly string METHOD_IS_DAYDREAM_READY = "isDaydreamReadyPlatform";

        private bool isDaydream = false;
        public bool IsDaydream
        {
            get
            {
                return isDaydream;
            }
        }

        private readonly string CARDBOARD_DEVICE_NAME = "cardboard";
        private readonly string DAYDREAM_DEVICE_NAME = "daydream";

        private Player player;
        private GameObject reticlePointer;
        private GameObject controllerMain;
        private GameObject controllerPointer;

#if !RUNNING_ON_ANDROID_DEVICE
        public enum EmulatedPlatformType
        {
            Daydream,
            Cardboard
        }

        [Tooltip("Emulated GVR Platform")]
        public EmulatedPlatformType gvrEmulatedPlatformType = EmulatedPlatformType.Daydream;
        public static string EMULATED_PLATFORM_PROP_NAME = "gvrEmulatedPlatformType";
#else
    // Running on an Android device.
    private GvrSettings.ViewerPlatformType viewerPlatform;
#endif  // !RUNNING_ON_ANDROID_DEVICE

        // private IEnumerator SwitchToVR()
        // {
        //     yield return null;
        //     string viewerPlatform = UnityEngine.VR.VRSettings.supportedDevices[1];
        //     Debug.Log("viewerPlatform : " + viewerPlatform);
        //     UnityEngine.VR.VRSettings.LoadDeviceByName(viewerPlatform);
        // }

        void Awake()
        {
            // StartCoroutine(SwitchToVR());

#if !RUNNING_ON_ANDROID_DEVICE
            if (playerSettingsHasDaydream() || playerSettingsHasCardboard())
            {
                // The list is populated with valid VR SDK(s), pick the first one.
                gvrEmulatedPlatformType =
                (UnityEngine.VR.VRSettings.supportedDevices[0] == DAYDREAM_DEVICE_NAME) ?
                EmulatedPlatformType.Daydream :
                EmulatedPlatformType.Cardboard;
            }
            isDaydream = (gvrEmulatedPlatformType == EmulatedPlatformType.Daydream);
#else
            // Running on an Android device.
            viewerPlatform = GvrSettings.ViewerPlatform;
            Debug.Log("viewerPlatform(GvrSettings.ViewerPlatform): " + viewerPlatform);
            // First loaded device in Player Settings.
            string vrDeviceName = UnityEngine.VR.VRSettings.loadedDeviceName;
            if (vrDeviceName != CARDBOARD_DEVICE_NAME &&
                vrDeviceName != DAYDREAM_DEVICE_NAME) 
            {
            Debug.LogErrorFormat("Loaded device was '{0}', must be one of '{1}' or '{2}'",
                    vrDeviceName, DAYDREAM_DEVICE_NAME, CARDBOARD_DEVICE_NAME);
            return;
            }

            Debug.Log("IsDeviceDaydreamReady:" + IsDeviceDaydreamReady());
            // On a non-Daydream ready phone, fall back to Cardboard if it's present in the list of
        // enabled VR SDKs.
        // On a Daydream-ready phone, go into Cardboard mode if it's the currently-paired viewer.
        if ((!IsDeviceDaydreamReady() && playerSettingsHasCardboard()) ||
            (IsDeviceDaydreamReady() && playerSettingsHasCardboard() &&
            GvrSettings.ViewerPlatform == GvrSettings.ViewerPlatformType.Cardboard)) {
        vrDeviceName = CARDBOARD_DEVICE_NAME;
        }
        isDaydream = (vrDeviceName == DAYDREAM_DEVICE_NAME);
#endif  // !RUNNING_ON_ANDROID_DEVICE
        // SetVRInputMechanism();
        }

#if RUNNING_ON_ANDROID_DEVICE
  // Running on an Android device.
  private static bool IsDeviceDaydreamReady() {
    // Check API level.
    using (var version = new AndroidJavaClass(PACKAGE_BUILD_VERSION)) {
      if (version.GetStatic<int>(FIELD_SDK_INT) < ANDROID_MIN_DAYDREAM_API) {
        return false;
      }
    }
    // API level > 24, check whether the device is Daydream-ready..
    AndroidJavaObject androidActivity = null;
    try {
      androidActivity = GvrActivityHelper.GetActivity();
    } catch (AndroidJavaException e) {
      Debug.LogError("Exception while connecting to the Activity: " + e);
      return false;
    }
    AndroidJavaClass daydreamApiClass = new AndroidJavaClass(PACKAGE_DAYDREAM_API_CLASS);
    if (daydreamApiClass == null || androidActivity == null) {
      return false;
    }
    return daydreamApiClass.CallStatic<bool>(METHOD_IS_DAYDREAM_READY, androidActivity);
  }
#endif  // RUNNING_ON_ANDROID_DEVICE

        private bool playerSettingsHasDaydream()
        {
            string[] playerSettingsVrSdks = UnityEngine.VR.VRSettings.supportedDevices;
            return Array.Exists<string>(playerSettingsVrSdks,
                element => element.Equals(DemoInputManager.DAYDREAM_DEVICE_NAME));
        }

        private bool playerSettingsHasCardboard()
        {
            string[] playerSettingsVrSdks = UnityEngine.VR.VRSettings.supportedDevices;
            return Array.Exists<string>(playerSettingsVrSdks,
                element => element.Equals(DemoInputManager.CARDBOARD_DEVICE_NAME));
        }

        public void SetVRInputMechanism()
        {
            SetGazeInputActive(!isDaydream);
            SetControllerInputActive(isDaydream);
        }

        private void SetGazeInputActive(bool active)
        {
            if (reticlePointer == null)
            {
                return;
            }
            reticlePointer.SetActive(active);

            // Update the pointer type only if this is currently activated.
            if (!active)
            {
                return;
            }

            GvrReticlePointer pointer =
                reticlePointer.GetComponent<GvrReticlePointer>();
            if (pointer != null)
            {
                GvrPointerInputModule.Pointer = pointer;
            }
        }

        private void SetControllerInputActive(bool active)
        {
            if (controllerMain != null)
            {
                controllerMain.SetActive(active);
            }
            if (controllerPointer == null)
            {
                return;
            }
            controllerPointer.SetActive(active);

            // Update the pointer type only if this is currently activated.
            if (!active)
            {
                return;
            }
            GvrLaserPointer pointer =
                controllerPointer.GetComponentInChildren<GvrLaserPointer>(true);
            if (pointer != null)
            {
                GvrPointerInputModule.Pointer = pointer;
            }
        }
    }
}