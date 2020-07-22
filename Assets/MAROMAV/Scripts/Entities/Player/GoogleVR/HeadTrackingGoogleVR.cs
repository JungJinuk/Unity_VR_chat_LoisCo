using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class HeadTrackingGoogleVR : HeadTracking
    {
        private Transform GvrHead
        {
            get
            {
                if (cashedGvrHead == null)
                {
                    cashedGvrHead = GameModel.Instance.CurrentPlayer.CameraRig.transform;
                }
                return cashedGvrHead;
            }
        }
        private Transform cashedGvrHead;

        void Update()
        {
            #if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
            transform.localRotation = GvrHead.rotation;
            #endif
        }
    }
}
