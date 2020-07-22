using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class PictureGallery : MonoBehaviour, IPageProvider
    {
        AndroidJavaClass myCls;
        AndroidJavaObject myObj;
        private List<string> galleryImages;

        [SerializeField]
        private GameObject framePrefab; 

        [SerializeField]
        GameObject oneElementPagePrefab;

        [SerializeField]
        private int galleryPageNum = 15;

        public List<GameObject> cashedGallery = new List<GameObject>();

        public List<byte[]> cashedFileData = new List<byte[]>();

        /// The spacing between pages in local coordinates.
        [Tooltip("The spacing between pages.")]
        public float spacing = 1000.0f;

        public float GetSpacing()
        {
            return spacing;
        }

        public int GetNumberOfPages()
        {
            return cashedGallery.Count;
        }

        public RectTransform ProvidePage(int index)
        {
            GameObject pageTransform = GameObject.Instantiate(cashedGallery[index]);
            RectTransform page = pageTransform.GetComponent<RectTransform>();

            Vector2 middleAnchor = new Vector2(0.5f, 0.5f);
            page.anchorMax = middleAnchor;
            page.anchorMin = middleAnchor;

            return page;
        }

        public void RemovePage(int index, RectTransform page)
        {
            GameObject.Destroy(page.gameObject);
        }

        /// When the page is cached, it will only be instantiated the first
        /// time the tab is opened. On subsequent times it will just be
        /// activated/deactivated.
        [Tooltip("Cache the page when the tab is closed.")]
        [SerializeField]
        private bool cachePage;

        //  2D 오브젝트 선택창
        [SerializeField]
        private GameObject selectPreviewCanvasPrefab;

        /// Represents the tab's page.
        public GameObject Page { get; private set; }

        /// Returns true if the tab is open.
        public bool IsOpened { get; private set; }

        // public static PictureGallery Instance { get; private set; }

        void Awake()
        {
            // Instance = this;

            // 새로룬 오브젝트를 만들지 않고, com.unity3d.player.UnityPlayer의
            // static 멤버에 접근하기 위해 AndroidJavaClass를 사용한다.
            // (사실 Android UnityPlayer가 자동으로 인스턴스를 생성해준다)
            myCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // 그리고 static 필드인 currentActivity를 접근한다.
            // 그리고 이경우 AndroidJavaObject를 사용한다.
            // 이유는 실제 필드 타입이 android.app.Activity이고
            // 이건 java.lang.Object를 상속받는다.
            // 그리고, non-primitive 타입은 무조껀 AndroidJavaObject로 접근해야한다.
            // 예외 : strings.
            myObj = myCls.GetStatic<AndroidJavaObject>("currentActivity");

            galleryImages = GetAllGalleryImagePaths();
            StartCoroutine(MakeGalleryPagePrefabs());
        }

        // void OnDestroy()
        // {
        //     if (Page != null)
        //     {
        //         GameObject.Destroy(Page);
        //     }
        // }

        private IEnumerator MakeGalleryPagePrefabs()
        {
            if (galleryImages.Count < galleryPageNum)
            {
                galleryPageNum = galleryImages.Count;
            }

            for (int i = 0; i < galleryPageNum; ++i)
            {
                GameObject go = GameObject.Instantiate(oneElementPagePrefab, Vector3.back * 1000f, Quaternion.identity);
                Image photo = go.GetComponent<TiledPage>().images[0];
                byte[] fileData = myObj.Call<byte[]>("Thumbnail", galleryImages[galleryImages.Count - 1 - i]);
                Texture2D new_texture = new Texture2D(16, 16);

                new_texture.LoadImage(fileData);
                int size = Mathf.Min(new_texture.width, new_texture.height);

                photo.sprite = Sprite.Create(new_texture, new Rect((new_texture.width - size) / 2, (new_texture.height - size) / 2, size, size), new Vector2(0, 0));
                cashedFileData.Add(fileData);
                cashedGallery.Add(go);
                yield return null;
            }
        }

        private List<string> GetAllGalleryImagePaths()
        {
            List<string> results = new List<string>();
            HashSet<string> allowedExtesions = new HashSet<string>() { ".png", ".jpg", ".jpeg" };

            try
            {
                AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");

                // Set the tags for the data we want about each image.  This should really be done by calling; 
                //string dataTag = mediaClass.GetStatic<string>("DATA");
                // but I couldn't get that to work...

                const string dataTag = "_data";

                string[] projection = new string[] { dataTag };
                AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");

                string[] urisToSearch = new string[] { "EXTERNAL_CONTENT_URI", "INTERNAL_CONTENT_URI" };
                foreach (string uriToSearch in urisToSearch)
                {
                    AndroidJavaObject externalUri = mediaClass.GetStatic<AndroidJavaObject>(uriToSearch);
                    AndroidJavaObject finder = currentActivity.Call<AndroidJavaObject>("managedQuery", externalUri, projection, null, null, null);
                    bool foundOne = finder.Call<bool>("moveToFirst");
                    while (foundOne)
                    {
                        int dataIndex = finder.Call<int>("getColumnIndex", dataTag);
                        string data = finder.Call<string>("getString", dataIndex);
                        if (allowedExtesions.Contains(Path.GetExtension(data).ToLower()))
                        {
                            string path = /*@"file:///" +*/ data;
                            results.Add(path);
                        }

                        foundOne = finder.Call<bool>("moveToNext");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                // do something with error...
            }

            return results;
        }

        public void OnSelectedPhoto()
        {
            PhotoToWorld();
            ControllerInteractionGoogleVR.controllerMode = ControllerInteractionGoogleVR.ControllerMode.HAND;
            SetOpenPreviewGalleryUI(false);
        }

        private void PhotoToWorld()
        {
            int activeRealIndex = Page.GetComponent<PagedScrollRect>().ActiveRealIndex;

            if (activeRealIndex < 0)
            {
                activeRealIndex = (activeRealIndex % galleryPageNum) + galleryPageNum;
            }
            else
            {
                activeRealIndex = activeRealIndex % galleryPageNum;
            }

            ControllerInteractionGoogleVR controllerInteraction = ControllerInteractionGoogleVR.Instance;
            GameObject frame = PhotonNetwork.Instantiate(framePrefab.name, controllerInteraction.laserPointer.GetPointAlongPointer(controllerInteraction.manipulateObjDistance), Quaternion.identity, 0);

            frame.GetComponent<Photo>().OnSelectPhoto(cashedFileData[activeRealIndex]);
        }

        public void SetOpenPreviewGalleryUI(bool open)
        {
            if (IsOpened == open)
            {
                return;
            }

            if (open)
            {
                EnablePreviewGallery();

                // Transition In
                IUITransition transition = FindTransition();
                if (transition != null)
                {
                    transition.TransitionIn(Page.transform, null, null);
                }
            }
            else
            {
                // Transition Out
                IUITransition transition = FindTransition();
                if (transition != null)
                {
                    transition.TransitionOut(Page.transform, () =>
                    {
                        DisablePreviewGallery();
                    }, null);
                }
                else
                {
                    DisablePreviewGallery();
                }
            }

            IsOpened = open;
        }

        private void EnablePreviewGallery()
        {
            Transform selectUIRoot = GameModel.Instance.CurrentPlayer.SelectUIRoot;
            if (Page != null)
            {
                Page.SetActive(true);
            }
            else
            {
                Page = GameObject.Instantiate(selectPreviewCanvasPrefab);
                Page.transform.SetParent(selectUIRoot, false);
            }
        }

        private void DisablePreviewGallery()
        {
            // If we are caching the page, then
            // just deactivate it. Otherwise, destroy it.
            if (cachePage)
            {
                Page.SetActive(false);
            }
            else
            {
                GameObject.Destroy(Page);
                Page = null;
            }
        }

        private IUITransition FindTransition()
        {
            return GameModel.Instance.CurrentPlayer.SelectUIRoot.GetComponent<IUITransition>();
        }
    }
}

