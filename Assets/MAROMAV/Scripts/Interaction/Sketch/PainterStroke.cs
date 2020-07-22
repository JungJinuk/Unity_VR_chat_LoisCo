using UnityEngine;
using UnityEngine.EventSystems;

namespace MAROMAV.CoworkSpace
{

    /// An individual stroke of paint.
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class PainterStroke : MonoBehaviour,
                                 IPointerEnterHandler,
                                 IPointerExitHandler
    {
        private MeshFilter meshFilter;

        public bool isHovered;
        public bool isClicking;
        public Painter painter;
        public Painter.Stroke stroke;

        public void Init(Painter _painter, Painter.Stroke _stroke)
        {
            painter = _painter;
            stroke = _stroke;
        }

        void Awake()
        {
            isHovered = false;
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
        }

        void Update()
        {
            if (painter.IsEraser && GvrControllerInput.ClickButtonDown &&
                !painter.menuRoot.IsMenuOpen())
            {
                isClicking = true;
            }
            else if (GvrControllerInput.ClickButtonUp)
            {
                isClicking = false;
            }
            if (painter.IsEraser && isClicking)
            {
                GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
                if (isHovered)
                {
                    painter.RemoveStroke(stroke);
                }
            }
        }

        void OnDestroy()
        {
            Destroy(meshFilter.sharedMesh);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
    }
}
