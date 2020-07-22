using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class HomeBackButtonScript : MonoBehaviour
    {

        public GameObject androidWebview;
        public Text url;
        bool isReady;
        string temp;
        
        public void TapReady(string function)
        {
            temp = function;
            isReady = true;
            StartCoroutine(ReadyTap());
        }
        public void SetIsReadyFalse()
        {
            isReady = false;
        }
        IEnumerator ReadyTap()
        {
            while (isReady && GvrPointerInputModule.Pointer.CurrentRaycastResult.gameObject.name.Contains(temp))
            {
                if (GvrControllerInput.TouchDown)
                {
                    if (temp.Equals("Home"))
                    {
                        androidWebview.GetComponent<AndroidWebview>().LoadUrlHome ("https://www.baidu.com");
                        isReady = false;
                    }
                    else if (temp.Equals("Back"))
                    {
                        androidWebview.GetComponent<AndroidWebview>().GoBack();
                        isReady = false;
                    }
                    else if(temp.Equals("Search")){
                        if(url.text.Contains("https://")){
                            androidWebview.GetComponent<AndroidWebview>().LoadUrl(url.text);
                        }else{
                            androidWebview.GetComponent<AndroidWebview>().LoadUrl("https://"+url.text);
                        } 
                        isReady=false;
                    }

                }
                yield return null;
            }

            isReady = false;
        }

    }
}