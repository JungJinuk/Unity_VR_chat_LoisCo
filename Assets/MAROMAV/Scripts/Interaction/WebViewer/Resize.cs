using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MAROMAV.CoworkSpace
{
    public class Resize : MonoBehaviour
    {
        public GameObject forResized;
        public float showSize;

        public float compensate;
        Vector3 sizeOfForResized;
        Vector3 sizeOfMe;
        Vector3 orgPos;
        bool isHovering;
        public bool isResizing;
        // Use this for initialization
        bool isIdle;
        public float showScale;
        public float hideScale;
        public float downLimit;
        public float upLimit;
        void Start()
        {
            sizeOfForResized = forResized.GetComponent<Renderer>().bounds.size;
            sizeOfMe = GetComponent<Renderer>().bounds.size;
            transform.position = forResized.transform.position + forResized.transform.right * (-0.1f+sizeOfForResized.x / 2) + forResized.transform.up * ( 0.1f-sizeOfForResized.y / 2) + forResized.transform.forward * 0.1f;
            orgPos = transform.position;
            isIdle = true;
        }

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
            float xScale = Mathf.Clamp((2 + transform.parent.transform.localScale.x) / 2, downLimit, upLimit);
            float yScale = Mathf.Clamp((2 + transform.parent.transform.localScale.y) / 2, downLimit, upLimit);
            transform.localScale = new Vector3(xScale * showScale, yScale * showScale, transform.localScale.z);
        }
        void Hide()
        {
            isIdle = true;
            //아이들 상태
            float xScale = Mathf.Clamp((2 + transform.parent.transform.localScale.x) / 2, downLimit, upLimit);
            float yScale = Mathf.Clamp((2 + transform.parent.transform.localScale.y) / 2, downLimit, upLimit);
            transform.localScale = new Vector3(xScale * hideScale, yScale * hideScale, transform.localScale.z);
        }
        public void SetIsHovering(bool hovering)
        {
            isHovering = hovering;
        }




        public void StartResize()
        {
            isResizing = true;

            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ResizeObject());
        }
        IEnumerator ResizeObject()
        {
            Vector3 originScale = forResized.transform.localScale;
            Vector3 limitScale = originScale * 0.5f;
            // float disNow;
            while (GvrControllerInput.ClickButton && !GvrControllerInput.ClickButtonUp)
            {

                Vector3 tempScale = forResized.transform.localScale + originScale * compensate * (Mathf.Clamp((GvrControllerInput.Gyro.x + GvrControllerInput.Gyro.y) / 2, -1, 1));
                if (tempScale.x < limitScale.x)
                {
                    forResized.transform.localScale = limitScale;
                }
                else
                {
                    forResized.transform.localScale = tempScale;
                }
                yield return null;
            }
            ReArrange();
        }

        public void ReArrange()
        {
            sizeOfForResized = forResized.GetComponent<Renderer>().bounds.size;
            orgPos = transform.position;
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            //forResized.GetComponent<ControlSub>().Arrange();
            isResizing = false;
        }

        public void SetResizerBasic()
        {
            isHovering = false;
            Hide();
        }
    }
}

