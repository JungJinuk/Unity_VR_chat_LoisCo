using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MAROMAV.CoworkSpace
{
    public class ControlSub : MonoBehaviour
    {
        // public GameObject home;
        // public GameObject back;
        public GameObject homeTop;
        public GameObject backTop;
        public GameObject homeDesign;
        public GameObject backDesign;
        public GameObject searchDesign;
        public GameObject transparent;
        bool isHovering;
        public bool isResizing;
        bool isIdle;

        // Update is called once per frame
        void Update()
        {
            if (isIdle)
            {
                if (isHovering)
                {
                    Show();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (isHovering)
                {
                    return;
                }
                else
                {
                    Hide();
                }
            }
        }

        void Show()
        {
            isIdle = false;
            //쇼 상태
            homeDesign.SetActive(true);
            backDesign.SetActive(true);
            searchDesign.SetActive(true);
            homeTop.GetComponent<Collider>().enabled = true;
            backTop.GetComponent<Collider>().enabled = true;
            transparent.GetComponent<Collider>().enabled = true;

        }

        void Hide()
        {
            isIdle = true;
            homeDesign.SetActive(false);
            backDesign.SetActive(false);
            searchDesign.SetActive(false);
            homeTop.GetComponent<Collider>().enabled = false;
            backTop.GetComponent<Collider>().enabled = false;
            transparent.GetComponent<Collider>().enabled = false;

            //아이들 상태
        }
        public void SetIsHovering(bool hovering)
        {
            isHovering = hovering;
        }
    }
}

