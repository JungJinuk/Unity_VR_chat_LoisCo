using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
//using UnityEditor;

namespace MAROMAV.CoworkSpace
{
    public class GalleryMaster : MonoBehaviour
    {
        AndroidJavaClass myCls;
        AndroidJavaObject myObj;
        private List<string> galleryImages;

        
        public int galleryPage = 0;
        public GameObject galleryCanvasPrefab;
        private GameObject galleryCanvas;
        public GameObject galleryPrefab;
        public GameObject[] gallery;
        public GameObject framePrefab;

        private Vector2 prePos;
        private float sensitivity;
        private bool isSwiping;
        private bool isLoading;
        private bool isTouching;
        private bool isGalleryLoaded;


        private static int galleryPageNum = 15;//odd num
                                               //private static float  = 1000f;
        private static float gallerySpacing = 4f;

        private static float scaleCenterGallery = 0.04f;
        private static float scaleSideGallery = 0.02f;

        public Sprite invisible;
        private float frameD = 0f;

        //static byte[] fileData;

        void Awake()
        {
            // if (GameModel.Instance.ActiveGameState is MainMenuGameState)
            // {
            //     return;
            // }
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

            //myObj = new AndroidJavaObject( "com.maromav.loiscophoto.LoisCoPhotoActivity" );


            galleryImages = GetAllGalleryImagePaths();

            galleryCanvas = Instantiate(galleryCanvasPrefab);
            gallery = new GameObject[galleryPageNum];

            Transform uiRoot = GameModel.Instance.CurrentPlayer.UIRoot;
            for (int i = 0; i < galleryPageNum; i++)
            {
                gallery[i] = Instantiate(galleryPrefab, new Vector3(i > galleryPageNum / 2 ? gallerySpacing * (i - galleryPageNum) : gallerySpacing * i, -15f, 10f), Quaternion.identity, galleryCanvas.transform);
                if (i == galleryPage) gallery[i].transform.localScale = Vector3.one * scaleCenterGallery;
                else if (i == galleryPageNum - 1 || i == 1) gallery[i].transform.localScale = Vector3.one * scaleSideGallery;
                else gallery[i].transform.localScale = Vector3.one * 0f;
            }
            //Debug.Log(myObj.Call<string>("StringTest"));
            //fileData = myObj.Call<byte[]>("Thumbnail", galleryImages[galleryImages.Count - 1]);
            //Debug.Log(fileData);
            StartCoroutine(LoadImageInit());
        }

        public IEnumerator LoadImageInit()
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < Mathf.Min(galleryPageNum / 2 + 1, galleryImages.Count); i++)
            {
                Image photo = gallery[i].GetComponent<Image>();
                // WWW www;
                // www = new WWW("file://" + galleryImages[galleryImages.Count - 1 - i]);
                // while (www.progress < 1f)
                // {

                //     yield return null;
                // }
                // yield return www;
                byte[] fileData = myObj.Call<byte[]>("Thumbnail", galleryImages[galleryImages.Count - 1 - i]);
                Texture2D new_texture = new Texture2D(16, 16);

                new_texture.LoadImage(fileData);
                Debug.Log("file length:"+fileData.Length);
                //photo.sprite = Sprite.Create(new_texture, new Rect(0, 0, new_texture.width, new_texture.height), new Vector2(0, 0));
                //www.LoadImageIntoTexture(new_texture);
                int size = Mathf.Min(new_texture.width, new_texture.height);

                photo.sprite = Sprite.Create(new_texture, new Rect((new_texture.width - size) / 2, (new_texture.height - size) / 2, size, size), new Vector2(0, 0));
                yield return null;
            }
        }

        void Update()
        {
            if (ControllerInteractionGoogleVR.controllerMode == ControllerInteractionGoogleVR.ControllerMode.SELECTING_PICTURE)
            {
                if (GvrControllerInput.AppButtonDown)
                {
                    GalleryButton();
                }

                if (GvrControllerInput.ClickButtonDown && isGalleryLoaded)
                {
                    StartCoroutine(Photo2World());
                }

                if (GvrControllerInput.IsTouching)//Swipe
                {
                    if (isTouching) sensitivity = GvrControllerInput.TouchPos.x - prePos.x;

                    prePos = GvrControllerInput.TouchPos;
                    if (sensitivity > 0.02f && gallery[0] != null && !isSwiping && isGalleryLoaded && galleryPage > 0)
                    {
                        StartCoroutine(SwipeGallery(SwipeDir.Right));
                    }
                    else if (sensitivity < -0.02f && gallery[0] != null && !isSwiping && isGalleryLoaded && galleryPage < galleryImages.Count - 1)
                    {
                        StartCoroutine(SwipeGallery(SwipeDir.Left));
                    }
                    isTouching = true;
                }
                else
                {
                    prePos = Vector2.zero;
                    sensitivity = 0f;
                    isTouching = false;
                }
            }
        }

        public void GalleryButton()
        {
            if (!isGalleryLoaded && !isLoading)
            {
                StartCoroutine(LoadGallery());
            }
            else if (isGalleryLoaded && !isLoading)
            {
                ControllerInteractionGoogleVR.controllerMode = ControllerInteractionGoogleVR.ControllerMode.HAND;
                StartCoroutine(UnloadGallery());
            }
        }

        public IEnumerator LoadGallery()
        {
            isLoading = true;
            iTween.MoveBy(galleryCanvas, iTween.Hash("y", 15f, "time", 0.2f, "easeType", "easeInOutExpo"));

            yield return new WaitForSeconds(0.2f);

            isLoading = false;
            isGalleryLoaded = true;
        }

        public IEnumerator UnloadGallery()
        {
            isLoading = true;

            iTween.MoveBy(galleryCanvas, iTween.Hash("y", -15f, "time", 0.2f, "easeType", "easeInOutExpo"));

            yield return new WaitForSeconds(0.2f);
            isLoading = false;
            isGalleryLoaded = false;
        }

        public IEnumerator LoadImage(int index, bool isLeft, bool isInvisible)
        {
            if (isInvisible)
            {
                Image photo = gallery[index].GetComponent<Image>();
                photo.sprite = invisible;
                yield return null;
            }
            else
            {
                Image photo = gallery[index].GetComponent<Image>();
                photo.sprite = invisible;
                 byte[] fileData = myObj.Call<byte[]>("Thumbnail", galleryImages[galleryImages.Count - 1 - galleryPage - (isLeft ? -galleryPageNum / 2 : galleryPageNum / 2)]);
                Texture2D new_texture = new Texture2D(16, 16);

                new_texture.LoadImage(fileData);
                // WWW www = new WWW("file://" + galleryImages[galleryImages.Count - 1 - galleryPage - (isLeft ? -galleryPageNum / 2 : galleryPageNum / 2)]);
                // while (www.progress < 1f)
                // {

                //     //Debug.Log("/progress:" + www.progress + "/isDone:" + www.isDone);
                //     yield return null;
                // }
                // yield return www;
                // Texture2D new_texture = new Texture2D(16, 16);
                // www.LoadImageIntoTexture(new_texture);

                //Debug.Log(new_texture.width + "/a/" + new_texture.height);

                //byte[] fileData;
                //do
                //{
                //    fileData = File.ReadAllBytes(galleryImages[galleryImages.Count - num++]);
                //    Debug.Log(fileData.Length);
                //    yield return null;
                //} while (fileData.Length > 1000000);

                //new_texture = new Texture2D(2, 2);

                //new_texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.


                int size = Mathf.Min(new_texture.width, new_texture.height);

                photo.sprite = Sprite.Create(new_texture, new Rect((new_texture.width - size) / 2, (new_texture.height - size) / 2, size, size), new Vector2(0, 0));
            }
        }


        public IEnumerator Photo2World()
        {
            GameObject frame = PhotonNetwork.Instantiate(framePrefab.name, ControllerInteractionGoogleVR.Instance.laserPointer.GetPointAlongPointer(5f), Quaternion.identity, 0);

            byte[] fileData = myObj.Call<byte[]>("Thumbnail",galleryImages[galleryImages.Count - 1 - galleryPage]);

            //  byte 배열의 파일 데이터를 만들어진 photo에 전달해서 photo에서 텍스쳐를 입히게 한다.
            frame.GetComponent<Photo>().OnSelectPhoto(fileData);

            ControllerInteractionGoogleVR.controllerMode = ControllerInteractionGoogleVR.ControllerMode.HAND;


            //PhotonView picturePhotonView = frame.GetComponent<PhotonView>();
            //picturePhotonView.RPC("OnSelectPhoto", PhotonTargets.All, fileData);

            //Debug.Log("bt:" + fileData);
            //Texture2D new_texture = new Texture2D(16, 16);
            //new_texture.LoadImage(fileData);
            // Debug.Log("tx:" + new_texture);

            //WWW www = new WWW("file://" + galleryImages[galleryImages.Count - 1 - galleryPage]);
            //while (www.progress < 1f)
            //{

            //    Debug.Log("/progress:" + www.progress + "/isDone:" + www.isDone);
            //    yield return null;
            //}
            //yield return www;
            //Texture2D new_texture = new Texture2D(16, 16);
            //www.LoadImageIntoTexture(new_texture);

            //int size = Mathf.Min(new_texture.width, new_texture.height);
            //frame.transform.localScale = new Vector3(5 * new_texture.width / size, 5 * new_texture.height / size, 1f);
            //frame.transform.GetComponent<Renderer>().material.mainTexture = new_texture;

            StartCoroutine(UnloadGallery());
            yield return new WaitForSeconds(0.2f);
        }

        public enum SwipeDir
        {
            Left,
            Right
        }

        public IEnumerator SwipeGallery(SwipeDir dir)
        {
            float move = dir == SwipeDir.Right ? gallerySpacing : -gallerySpacing;
            isSwiping = true;
            galleryPage += dir == SwipeDir.Right ? -1 : 1;

            for (int j = 0; j < galleryPageNum; j++)
            {
                if (dir == SwipeDir.Right && j == (1 + galleryPage + galleryPageNum / 2) % galleryPageNum)
                {
                    gallery[j].transform.Translate(-galleryPageNum * gallerySpacing, 0, 0);
                }
                else if (dir == SwipeDir.Left && j == (galleryPage + galleryPageNum / 2) % galleryPageNum)
                {
                    gallery[j].transform.Translate(galleryPageNum * gallerySpacing, 0, 0);
                }

                iTween.MoveBy(gallery[j], iTween.Hash("x", move, "time", 0.2f, "easeType", "easeInOutExpo"));

                if (j == galleryPage % galleryPageNum)
                {
                    iTween.ScaleTo(gallery[j], iTween.Hash("x", scaleCenterGallery, "y", scaleCenterGallery, "time", 0.2f, "easeType", "easeInOutExpo"));
                }
                else if (j == (galleryPage + 1) % galleryPageNum || j == (galleryPage + galleryPageNum - 1) % galleryPageNum)
                {
                    iTween.ScaleTo(gallery[j], iTween.Hash("x", scaleSideGallery, "y", scaleSideGallery, "time", 0.2f, "easeType", "easeInOutExpo"));
                }
                else
                {
                    iTween.ScaleTo(gallery[j], iTween.Hash("x", 0f, "y", 0f, "time", 0.2f, "easeType", "easeInOutExpo"));
                }
            }

            for (int j = 0; j < galleryPageNum; j++)
            {
                if (dir == SwipeDir.Right && j == (1 + galleryPage + galleryPageNum / 2) % galleryPageNum)
                {

                    if (galleryPage > galleryPageNum / 2 - 1) StartCoroutine(LoadImage(j, true, false));
                    else StartCoroutine(LoadImage(j, true, true));


                }
                else if (dir == SwipeDir.Left && j == (galleryPage + galleryPageNum / 2) % galleryPageNum)
                {
                    if (galleryPage < galleryImages.Count - galleryPageNum / 2) StartCoroutine(LoadImage(j, false, false));
                    else StartCoroutine(LoadImage(j, false, true));
                }
            }
            yield return new WaitForSeconds(0.25f);
            isSwiping = false;
            yield return null;
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
    }
}
