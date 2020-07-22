using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MAROMAV.CoworkSpace
{
    public class ClickMenuRoot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        private Vector3 menuCenter;
        private Quaternion menuOrientation;

        private ClickMenuIcon dummyParent;
        private bool selected;

        public delegate void MenuOpenedEvent();
        public event MenuOpenedEvent OnMenuOpened;

        public delegate void MenuClosedEvent();
        public event MenuClosedEvent OnMenuClosed;

        public delegate void ItemSelectedEvent(ClickMenuItem id);
        public event ItemSelectedEvent OnItemSelected;

        public delegate void ItemHoveredEvent(ClickMenuItem id);
        public event ItemHoveredEvent OnItemHovered;

        [Tooltip("The tree of menu items")]
        public ClickMenuTree menuTree;

        [Tooltip("(Optional) The center icon")]
        public Sprite backIcon;

        [Tooltip("(Optional) Moves reticle closer on hover")]
        public ObjectManipulationPointer laserPointer;

        [Tooltip("(Optional) Background material for the menu")]
        public Material pieMaterial;

        [Tooltip("Scale applied to all the icons")]
        public float iconScale = .1f;

        public enum GvrMenuActivationButton
        {
            ClickButtonDown,
            ClickButtonUp,
            AppButtonDown,
            AppButtonUp
        }

        [Tooltip("Input event to trigger the menu")]
        public GvrMenuActivationButton menuActivationButton = GvrMenuActivationButton.ClickButtonDown;

        [Tooltip("Prefab used for each item in the menu")]
        public ClickMenuIcon menuIconPrefab;

        [Tooltip("Maximum number of meters the reticle can move per second.")]
        [Range(0.1f, 50.0f)]
        public float reticleDelta = 10.0f;

        [Tooltip("Distance away from the controller of the menu in meters.")]
        [Range(0.6f, 5.0f)]
        public float menuDistance = 0.75f;

        [Tooltip("Angle away from the menu center to cause a closure.")]
        [Range(20.0f, 40.0f)]
        public float closeAngle = 25.0f;

        [Tooltip("Angle of gaze vs pointer needed to open a menu.")]
        [Range(30.0f, 50.0f)]
        public float openFovAngle = 35.0f;

        /// Scale factor to apply to the menuDistance used to
        /// determine the max distance of the pointer. Without this scale factor,
        /// the max distance will fall short of the menu by an increasing amount as the
        /// pointer moves away from the center of the menu.
        private const float POINTER_DISTANCE_SCALE = 1.0f;

        public GameObject LastCashedRaycastHitContent
        {
            get
            {
                if (lastCashedRaycastHitContent != null)
                {
                    return lastCashedRaycastHitContent;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    if (value.transform.root.gameObject.CompareTag("MoveablePhysicsObject"))
                    {
                        lastCashedRaycastHitContent = value.transform.root.gameObject;
                    }
                    else
                    {
                        lastCashedRaycastHitContent = null;
                    }
                }
                else
                {
                    lastCashedRaycastHitContent = null;
                }
            }
        }
        private GameObject lastCashedRaycastHitContent;


        void Awake()
        {
            selected = false;
        }

        public bool IsMenuOpen()
        {
            return dummyParent != null;
        }

        /// Determine if the activation button is held down.
        private bool IsButtonClicked()
        {
            switch (menuActivationButton)
            {
                case GvrMenuActivationButton.ClickButtonDown:
                    return GvrControllerInput.ClickButtonDown;
                case GvrMenuActivationButton.ClickButtonUp:
                    return GvrControllerInput.ClickButtonUp;
                case GvrMenuActivationButton.AppButtonDown:
                    return GvrControllerInput.AppButtonDown;
                case GvrMenuActivationButton.AppButtonUp:
                    return GvrControllerInput.AppButtonUp;
                default:
                    return false;
            }
        }

        private void SetMenuLocation()
        {
            Vector3 laserEndPt = laserPointer.GetPointAlongPointer(menuDistance);

            // Center and orient the menu
            menuCenter = laserEndPt;
            Vector3 headRight = Camera.main.transform.right;
            headRight.y = 0.0f;
            headRight.Normalize();
            Vector3 cameraCenter = Camera.main.transform.position;
            Vector3 rayFromCamera = (laserEndPt - cameraCenter).normalized;
            Vector3 up = Vector3.Cross(rayFromCamera, headRight);
            menuOrientation = Quaternion.LookRotation(rayFromCamera, up);
        }

        private bool IsMenuInFOV()
        {
            Vector3 cameraCenter = Camera.main.transform.position;
            Vector3 menuDirection = menuCenter - cameraCenter;
            Vector3 gazeDirection = Camera.main.transform.forward;

            return Vector3.Angle(menuDirection, gazeDirection) < openFovAngle;
        }

        private bool IsPointingAway()
        {
            if (!laserPointer.IsAvailable)
            {
                return true;
            }

            // Get the position and orientation form the arm model
            Vector3 laserEnd = laserPointer.MaxPointerEndPoint;

            Vector3 cameraCenter = Camera.main.transform.position;
            Vector3 menuCenterRelativeToCamera = menuCenter - cameraCenter;
            Vector3 laserEndRelativeToCamera = laserEnd - cameraCenter;

            float angle = Vector3.Angle(laserEndRelativeToCamera.normalized, menuCenterRelativeToCamera.normalized);
            return angle > closeAngle;
        }

        public void CloseAll()
        {
            selected = false;
            if (dummyParent)
            {
                dummyParent.CloseAll();
                Destroy(dummyParent.gameObject);
                dummyParent = null;
                if (OnMenuClosed != null)
                {
                    OnMenuClosed.Invoke();
                }
            }
        }

        void Update()
        {
            if (ControllerInteractionGoogleVR.controllerMode == ControllerInteractionGoogleVR.ControllerMode.SELECTING_PICTURE ||
                ControllerInteractionGoogleVR.Instance.pictureGallery.IsOpened)
            {
                return;
            }

            // Update the menu state if it needs to suddenly open or close
            if (!dummyParent && IsButtonClicked())
            {
                SetMenuLocation();
                if (IsMenuInFOV())
                {
                    LastCashedRaycastHitContent = laserPointer.CurrentRaycastResult.gameObject;

                    dummyParent = (ClickMenuIcon)Instantiate(menuIconPrefab, transform);
                    dummyParent.menuRoot = this;
                    ClickMenuIcon.ShowMenu(this, menuTree.tree.Root, dummyParent,
                                           menuCenter, menuOrientation, iconScale);
                    dummyParent.SetDummy();
                    if (OnMenuOpened != null)
                    {
                        OnMenuOpened.Invoke();
                    }
                }
            }
            else if ((GvrControllerInput.ClickButtonDown && !selected) ||
                     IsPointingAway())
            {
                CloseAll();
            }
            else if (dummyParent && GvrControllerInput.AppButtonUp)
            {
                MakeSelection(null);
                dummyParent.DeepestMenu().ShowParentMenu();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            selected = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selected = false;
        }

        public void MakeSelection(ClickMenuItem item)
        {
            if (OnItemSelected != null)
            {
                OnItemSelected.Invoke(item);
            }
        }

        public void MakeHover(ClickMenuItem item)
        {
            if (OnItemHovered != null)
            {
                OnItemHovered.Invoke(item);
            }
        }
    }
}
