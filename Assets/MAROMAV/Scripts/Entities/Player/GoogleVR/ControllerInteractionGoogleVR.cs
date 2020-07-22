using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    /// ControllerInteractionGoogleVR
    /// Daydream controller 의 인터렉션을 받아서
    /// 메뉴를 선택했을 경우를 처리하거나
    /// 스케치 모드일 경우 그림을 그릴 수 있게 한다
    public class ControllerInteractionGoogleVR : Photon.PunBehaviour
    {
        public ClickMenuRoot menuRoot;

        //  생성할 web view 프리팹. photon 으로 생성한다
        //  Resources 폴더 하위에 있어야 한다
        [SerializeField]
        private GameObject webviewPrefab;

        //  생성할 pdf select view 프리팹.
        //  Resources 폴더 하위에 있어야 한다
        [SerializeField]
        private GameObject pdfSelectPrefab;


        //  생성할 3D 오브젝트 모음
        public GameObject[] logoModels;

        [Tooltip("(Optional) Moves reticle closer on hover")]
        public ObjectManipulationPointer laserPointer;

        [Tooltip("Distance away from the controller of the menu in meters.")]
        [Range(2f, 10.0f)]
        public float manipulateObjDistance = 5f;

        public static ControllerInteractionGoogleVR Instance { get; private set; }

        public enum ControllerMode
        {
            NONE = 0,
            HAND,
            PEN,
            ERASER,
            SELECTING_PICTURE,
        }

        public static ControllerMode controllerMode = ControllerMode.HAND;

        [HideInInspector]
        public Painter painter;

        [HideInInspector]
        public PictureGallery pictureGallery;


        //  pointer 끝에 부딪힌 게임 오브젝트를 반환
        public GameObject RaycastHitObject
        {
            get
            {
                GameObject raycastHitObject = laserPointer.CurrentRaycastResult.gameObject;

                if (raycastHitObject != null)
                {
                    return raycastHitObject;
                }
                else
                {
                    return null;
                }
            }
        }

        //  스케치를 할 위치
        //  부딪힌 물체가 없다면 기본값의 reticle 위치에 그리고
        //  부딪힌 물체가 있다면 해당 위치의 99:1 위치에 (물체의 약간 앞) 그린다
        public Vector3 BrushPoint
        {
            get
            {
                if (laserPointer == null)
                {
                    return Vector3.zero;
                }

                Vector3 pointerEndPoint;

                if (RaycastHitObject != null)
                {
                    pointerEndPoint = ((laserPointer.CurrentRaycastResult.worldPosition * 99f) + laserPointer.GetPointAlongPointer(0f)) / 100f;
                }
                else
                {
                    pointerEndPoint = laserPointer.GetPointAlongPointer(laserPointer.defaultReticleDistance);
                }

                return pointerEndPoint;
            }
        }

        public Vector3 ReticlePosition
        {
            get
            {
                Vector3 reticleEndPoint;

                if (laserPointer.CurrentRaycastResult.gameObject != null)
                {
                    reticleEndPoint = laserPointer.CurrentRaycastResult.worldPosition;
                }
                else
                {
                    reticleEndPoint = laserPointer.GetPointAlongPointer(laserPointer.defaultReticleDistance);
                }

                return reticleEndPoint;
            }
        }

        public bool IsNowPointerInWebViewer
        {
            get
            {
                GameObject raycastHitObject = RaycastHitObject;

                if (raycastHitObject != null)
                {
                    if (raycastHitObject.CompareTag("WebViewer"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        void Update()
        {
            // Do not allow painting if the menu system is open
            if (menuRoot.IsMenuOpen())
            {
                painter.EndStroke();
                return;
            }

            if (controllerMode == ControllerMode.SELECTING_PICTURE && pictureGallery.IsOpened)
            {
                if (GvrControllerInput.AppButtonUp)
                {
                    pictureGallery.SetOpenPreviewGalleryUI(false);
                    controllerMode = ControllerMode.HAND;
                }
            }

            if (controllerMode == ControllerMode.PEN)
            {
                // Start, stop, or continue painting
                if (GvrControllerInput.ClickButtonDown)
                {
                    painter.StartStroke(BrushPoint);
                }
                else if (GvrControllerInput.ClickButtonUp)
                {
                    painter.EndStroke();
                }
                else if (GvrControllerInput.ClickButton)
                {
                    painter.ContinueStroke(BrushPoint);
                }
            }
        }

        void OnEnable()
        {
            menuRoot.OnItemSelected += OnItemSelected;
            painter = GetComponent<Painter>();
            pictureGallery = GetComponent<PictureGallery>();
            Instance = this;
        }

        void Start()
        {
            SetHandMode();
        }

        //  app button 메뉴 선택
        private void OnItemSelected(ClickMenuItem item)
        {
            int id = (item ? item.id : -1);
            switch (id)
            {
                //  delete content
                case 3:
                    DeleteContent();
                    break;

                //  add contents child menu
                //  select picture
                case 11:
                    SelectPicture();
                    break;
                //  web viewer
                case 12:
                    CreateWebViewer();
                    break;
                //  pdf viewer
                case 13:
                    CreatePDFViewer();
                    break;

                //  3d models child menu
                //  Google
                case 31:
                    CreateLogoModel(logoModels[0]);
                    break;
                //  Oculus
                case 32:
                    CreateLogoModel(logoModels[1]);
                    break;
                //  Vive
                case 33:
                    CreateLogoModel(logoModels[2]);
                    break;
                //  Microsoft
                case 34:
                    CreateLogoModel(logoModels[3]);
                    break;
                //  Samsung
                case 35:
                    CreateLogoModel(logoModels[4]);
                    break;


                //  tools child menu
                //  hand
                case 21:
                    SetHandMode();
                    break;
                //  pen
                case 22:
                    SetBrushPencil();
                    break;
                //  eraser
                case 23:
                    SetBrushEraser();
                    break;
                //  clear
                case 24:
                    Clear();
                    break;
            }
        }

        //  사진/웹/3D모델링 컨텐츠 삭제
        public void DeleteContent()
        {
            GameObject hoverContent = menuRoot.LastCashedRaycastHitContent;
            if (hoverContent != null)
            {
                PhotonNetwork.Destroy(hoverContent);
            }
        }

        //  사진 선택 화면
        private void SelectPicture()
        {
            //  기기에 사진이 하나도 없는 경우
            if (pictureGallery.cashedGallery.Count == 0)
            {
                GameView.Instance.ShowGamePopupPanel("No Picture !", "There are not any photos on your device.");
            }
            else
            {
                pictureGallery.SetOpenPreviewGalleryUI(true);
                controllerMode = ControllerMode.SELECTING_PICTURE;
                painter.SetHandMode();
            }
        }

        //  웹 생성
        private void CreateWebViewer()
        {
            //  이미 웹 뷰어가 한개 이상 있는 경우
            if (FindObjectsOfType<AndroidWebview>().Length != 0)
            {
                GameView.Instance.ShowGamePopupPanel("Webviewer is already exist! \n just one webviewer is supported.");
            }
            else
            {
                SetHandMode();
                Vector3 laserEndPt = laserPointer.GetPointAlongPointer(manipulateObjDistance);
                PhotonNetwork.Instantiate(webviewPrefab.name, laserEndPt, Quaternion.identity, 0);
            }
        }

        private void CreatePDFViewer()
        {
            SetHandMode();
            Vector3 laserEndPt = laserPointer.GetPointAlongPointer(manipulateObjDistance);
            GameObject.Instantiate(pdfSelectPrefab, laserEndPt, Quaternion.identity);
        }

        //  3d 모델 생성
        private void CreateLogoModel(GameObject logoGo)
        {
            SetHandMode();
            Vector3 laserEndPt = laserPointer.GetPointAlongPointer(manipulateObjDistance);
            PhotonNetwork.Instantiate(logoGo.name, laserEndPt, logoGo.transform.rotation, 0);
        }

        //  Hand
        private void SetHandMode()
        {
            controllerMode = ControllerMode.HAND;
            painter.SetHandMode();
        }

        //  스케치
        public void SetBrushPencil()
        {
            controllerMode = ControllerMode.PEN;
            painter.SetBrushPencil();
        }

        //  지우개
        public void SetBrushEraser()
        {
            controllerMode = ControllerMode.ERASER;
            painter.SetBrushEraser();
        }

        //  모든 스케치 삭제
        public void Clear()
        {
            painter.Clear();
        }
    }
}
