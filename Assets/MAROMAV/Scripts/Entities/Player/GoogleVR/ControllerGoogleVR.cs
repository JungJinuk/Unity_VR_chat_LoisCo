using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class ControllerGoogleVR : MonoBehaviour
    {
        public GvrTrackedController gvrTrackedController;
        public ControllerInteractionGoogleVR controllerInteractionGoogleVR;
        // public PictureGallery pictureGallery;

        void Awake()
        {
            var photonView = GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                // Destroy(GetComponent<GvrTrackedController>());
                // Destroy(GetComponent<ControllerInteractionGoogleVR>());
                // Destroy(GetComponent<PictureGallery>());
                Destroy(gvrTrackedController);
                Destroy(controllerInteractionGoogleVR);
                // Destroy(pictureGallery);
            }
        }
    }
}
