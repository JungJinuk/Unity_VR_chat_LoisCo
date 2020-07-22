using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MAROMAV.CoworkSpace
{
    public class EdgeResize : MonoBehaviour
    {
        GameObject forResized;
        public float showSize;

        public float compensate;
        Vector3 sizeOfForResized;
        Vector3 orgPos;
        bool isHovering;
        public bool isResizing;
        bool isIdle;
        public float showScale;
        public float hideScale;
        public float downLimit;
        public float upLimit;
        Vector2 directionFactor;
        public bool noRenderer;

        private bool IsHandMode
        {
            get
            {
                return ControllerInteractionGoogleVR.controllerMode == ControllerInteractionGoogleVR.ControllerMode.HAND;
            }
        }

        void Start()
        {
            directionFactor = new Vector2(0, 0);

            forResized = transform.parent.gameObject;
            if (!noRenderer)
                sizeOfForResized = forResized.GetComponent<Renderer>().bounds.size;
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
            if (!IsHandMode)
            {
                return;
            }

            isIdle = false;
            ControllerInteractionGoogleVR.Instance.painter.SetResizeMode();
        }

        void Hide()
        {
            isIdle = true;
            if (!isResizing)
            {
                ControllerInteractionGoogleVR.Instance.painter.SetHandMode();
            }
        }

        public void SetIsHovering(bool hovering)
        {
            isHovering = hovering;
        }

        public void StartResize(GameObject clickedEdge)
        {
            if (!IsHandMode)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                switch (clickedEdge.name)
                {
                    case "Up":
                        directionFactor = new Vector2(-1, 0);
                        break;
                    case "Down":
                        directionFactor = new Vector2(1, 0);
                        break;
                    case "Right":
                        directionFactor = new Vector2(0, 1);
                        break;
                    case "Left":
                        directionFactor = new Vector2(0, -1);
                        break;
                    case "UpRight":
                        directionFactor = new Vector2(-1, 1);
                        break;
                    case "UpLeft":
                        directionFactor = new Vector2(-1, -1);
                        break;
                    case "DownRight":
                        directionFactor = new Vector2(1, 1);
                        break;
                    case "DownLeft":
                        directionFactor = new Vector2(1, -1);
                        break;
                }

            }
            else
            {
                switch (clickedEdge.name)
                {
                    case "Up":
                        directionFactor = new Vector2(0, -1);
                        break;
                    case "Down":
                        directionFactor = new Vector2(0, 1);
                        break;
                    case "Right":
                        directionFactor = new Vector2(1, 0);
                        break;
                    case "Left":
                        directionFactor = new Vector2(-1, 0);
                        break;
                    case "UpRight":
                        directionFactor = new Vector2(1, -1);
                        break;
                    case "UpLeft":
                        directionFactor = new Vector2(-1, -1);
                        break;
                    case "DownRight":
                        directionFactor = new Vector2(1, 1);
                        break;
                    case "DownLeft":
                        directionFactor = new Vector2(-1, 1);
                        break;
                }
            }
            isResizing = true;
            if (!noRenderer)
            {
                clickedEdge.GetComponent<Collider>().enabled = false;

            }
            StartCoroutine(ResizeObject(clickedEdge));
        }

        IEnumerator ResizeObject(GameObject clickedEdge)
        {
            Vector3 originScale = forResized.transform.localScale;
            Vector3 upLimitScale = originScale * 0.5f;
            Vector3 downLimitScale = originScale * 1.5f;
            // float disNow;
            while (GvrControllerInput.ClickButton && !GvrControllerInput.ClickButtonUp)
            {

                Vector3 tempScale = forResized.transform.localScale + originScale * compensate * (Mathf.Clamp((GvrControllerInput.Gyro.x * directionFactor.x + GvrControllerInput.Gyro.y * directionFactor.y) / 2, -1, 1));
                if (tempScale.x < upLimitScale.x)
                {
                    forResized.transform.localScale = upLimitScale;
                }
                else if (tempScale.x > downLimitScale.x)
                {
                    forResized.transform.localScale = downLimitScale;
                }
                else
                {
                    forResized.transform.localScale = tempScale;
                }
                yield return null;
            }
            ReArrange(clickedEdge);
        }

        public void ReArrange(GameObject clickedEdge)
        {
            if (!noRenderer)
            {
                sizeOfForResized = forResized.GetComponent<Renderer>().bounds.size;
                clickedEdge.GetComponent<Collider>().enabled = true;
            }
            orgPos = transform.position;


            ControllerInteractionGoogleVR.Instance.painter.SetHandMode();
            isResizing = false;
        }

        public void SetResizerBasic()
        {
            isHovering = false;
            Hide();
        }
    }
}

