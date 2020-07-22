using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{

    /// <summary>
    /// Component's container for Player
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private Transform _uiRoot;
        public Transform UIRoot
        {
            get
            {
                return _uiRoot;
            }
        }

        [SerializeField]
        private Transform _selectUIRoot;
        public Transform SelectUIRoot
        {
            get
            {
                return _selectUIRoot;
            }
        }

        [SerializeField]
        private Transform _nameUiRoot;
        public Transform NameUIRoot
        {
            get
            {
                return _nameUiRoot;
            }
        }

        [SerializeField]
        private GameObject _clickMenuUIRoot;
        public GameObject ClickMenuUIRoot
        {
            get
            {
                return _clickMenuUIRoot;
            }
        }


        [SerializeField]
        GameObject _cameraRig;
        public GameObject CameraRig
        {
            get
            {
                return _cameraRig;
            }
        }

        [SerializeField]
        GameObject _head;
        public GameObject Head
        {
            get
            {
                return _head;
            }
        }

        [SerializeField]
        private GameObject _controllerPointer;
        public GameObject ControllerPointer
        {
            get
            {
                return _controllerPointer;
            }
        }

        [SerializeField]
        private GameObject _controllerMain;
        public GameObject ControllerMain
        {
            get
            {
                return _controllerMain;
            }
        }

        [SerializeField]
        private GameObject _reticlePointer;
        public GameObject ReticlePointer
        {
            get
            {
                return _reticlePointer;
            }
        }

        [SerializeField]
        private PhotonView _photonView;
        public PhotonView MyPhotonView
        {
            get
            {
                return _photonView;
            }
        }
    }
}
