using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Photon;

namespace MAROMAV.CoworkSpace
{
    /// The Painter manages a collection of paint strokes and is responsible
    /// for creating, modifying, and removing the strokes.
    public class Painter : PunBehaviour
    {
        public ClickMenuRoot menuRoot;

        // [Tooltip("(Optional) Moves reticle closer on hover")]
        // public ObjectManipulationPointer laserPointer;

        private const float PENCIL_THICKNESS = 0.009f;
        private const float MIN_VERTEX_DISPLACEMENT = 0.003f;

        public class Stroke
        {
            public GameObject obj;
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> indices = new List<int>();
            public Material savedMat;
        }


        private Vector3 lastPoint;
        private List<Stroke> strokes;
        private List<Stroke> savedStrokes;
        private Stroke curStroke;
        private Mesh curMesh;
        private MeshRenderer curMeshRenderer;
        private float brushThickness;
        private bool useControllerAngle;
        private Material paintMaterial;

        public bool IsEraser { get; private set; }

        private MaterialPropertyBlock propertyBlock;

        public MeshRenderer reticleRenderer;

        public Material cursorHand;
        public Material cursorGrab;
        public Material cursorPencil;
        public Material cursorEraser;
        public Material cursorResizer;

        private Material initialReticleMaterial;
        private Material currentBrushMaterial;

        private Stroke cashedStroke;

        public PhotonView PV
        {
            get
            {
                if (pv == null)
                {
                    pv = GetComponent<PhotonView>();
                    if (pv == null)
                    {
                        pv = this.photonView;
                    }
                }
                return pv;
            }
        }
        private PhotonView pv;

        void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            if (reticleRenderer != null)
            {
                initialReticleMaterial = reticleRenderer.sharedMaterial;
            }
            strokes = new List<Stroke>();

            int positionIndex = (int)PV.owner.CustomProperties["position"];
            paintMaterial = PlayerFactory.Instance.GetMaterialByPlayer(positionIndex);
        }

        public void UseInitialMaterial()
        {
            reticleRenderer.sharedMaterial = initialReticleMaterial;
        }

        public void UseBrushMaterial()
        {
            reticleRenderer.sharedMaterial = currentBrushMaterial;
        }

        public void SetMaterial(Material material)
        {
            paintMaterial = material;
            if (material && !IsEraser)
            {
                reticleRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", paintMaterial.color);
                reticleRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        public void SetHandMode()
        {
            PV.RPC("SetReticleHand", PhotonTargets.AllBuffered);
        }

        public void SetGrabMode()
        {
            PV.RPC("SetReticleGrab", PhotonTargets.AllBuffered);
        }

        public void SetBrushPencil()
        {
            PV.RPC("SetReticlePencil", PhotonTargets.AllBuffered);
        }

        public void SetBrushEraser()
        {
            PV.RPC("SetReticleEraser", PhotonTargets.AllBuffered);
        }

        public void SetResizeMode()
        {
            PV.RPC("SetReticleResizer", PhotonTargets.AllBuffered);
        }

        private void SetReticleMaterial(Material material)
        {
            reticleRenderer.sharedMaterial = material;
            currentBrushMaterial = material;
            reticleRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", paintMaterial.color);
            reticleRenderer.SetPropertyBlock(propertyBlock);
        }

        [PunRPC]
        private void SetReticleHand()
        {
            brushThickness = 0f;
            useControllerAngle = false;
            IsEraser = false;
            SetReticleMaterial(cursorHand);
        }

        [PunRPC]
        private void SetReticleGrab()
        {
            brushThickness = 0f;
            useControllerAngle = false;
            IsEraser = false;
            SetReticleMaterial(cursorGrab);
        }

        [PunRPC]
        private void SetReticlePencil()
        {
            brushThickness = PENCIL_THICKNESS;
            useControllerAngle = false;
            IsEraser = false;
            SetReticleMaterial(cursorPencil);
        }

        [PunRPC]
        private void SetReticleEraser()
        {
            brushThickness = 0.0f;
            useControllerAngle = false;
            IsEraser = true;
            SetReticleMaterial(cursorEraser);
        }

        [PunRPC]
        private void SetReticleResizer()
        {
            brushThickness = 0.0f;
            useControllerAngle = false;
            IsEraser = false;
            SetReticleMaterial(cursorResizer);
        }

        public void RemoveStroke(Stroke stroke)
        {
            cashedStroke = stroke;
            PV.RPC("RemoveStrokeRPC", PhotonTargets.AllBuffered);
        }

        [PunRPC]
        private void RemoveStrokeRPC()
        {
            if (strokes.Remove(cashedStroke))
            {
                Destroy(cashedStroke.obj);
            }
        }

        void OnDestory()
        {
            Clear();
        }

        private void CreateCurrentStroke()
        {
            curStroke = new Stroke();
            curStroke.obj = new GameObject("Stroke " + strokes.Count);
            curStroke.obj.transform.SetParent(transform.root, true);
            curStroke.obj.AddComponent<PainterStroke>().Init(this, curStroke);
            curMesh = curStroke.obj.GetComponent<MeshFilter>().sharedMesh;
            curMeshRenderer = curStroke.obj.GetComponent<MeshRenderer>();
            curMeshRenderer.sharedMaterial = paintMaterial;
        }

        public void StartStroke(Vector3 brushPoint)
        {
            PV.RPC("StartStrokeRPC", PhotonTargets.AllBuffered, brushPoint);
        }

        [PunRPC]
        private void StartStrokeRPC(Vector3 brushPoint)
        {
            if (!IsEraser)
            {
                lastPoint = brushPoint;
                CreateCurrentStroke();
                strokes.Add(curStroke);
            }
        }

        public void EndStroke()
        {
            PV.RPC("EndStrokeRPC", PhotonTargets.AllBuffered);
        }

        [PunRPC]
        private void EndStrokeRPC()
        {
            curStroke = null;
        }

        public void ContinueStroke(Vector3 brushPoint)
        {
            PV.RPC("ContinueStrokeRPC", PhotonTargets.AllBuffered, brushPoint);
        }

        [PunRPC]
        private void ContinueStrokeRPC(Vector3 brushPoint)
        {
            if (!IsEraser && curStroke != null)
            {
                AddNextVertex(brushPoint);
            }
        }

        private void UpdateCurMesh()
        {
            curMesh.vertices = curStroke.vertices.ToArray();
            curMesh.SetIndices(curStroke.indices.ToArray(), MeshTopology.Triangles, 0);
            curMesh.RecalculateBounds();
        }

        private void AddNextVertex(Vector3 brushPoint)
        {
            // Check if enough movement has occurred
            Vector3 newPoint = brushPoint;
            Vector3 delta = newPoint - lastPoint;

            if (delta.magnitude < MIN_VERTEX_DISPLACEMENT)
            {
                return;
            }

            // If this is the first time here, add, the base-vertex
            if (curStroke.vertices.Count.Equals(0))
            {
                Vector3 perpLast = GetPerpVector(delta, lastPoint);
                curStroke.vertices.Add(lastPoint + perpLast);
                curStroke.vertices.Add(lastPoint - perpLast);
            }

            // Add the next vertex
            Vector3 perp = GetPerpVector(delta, newPoint);
            curStroke.vertices.Add(lastPoint + perp);
            curStroke.vertices.Add(lastPoint - perp);

            // Add the next triangles
            int vIndex = curStroke.vertices.Count;
            curStroke.indices.Add(vIndex - 1);
            curStroke.indices.Add(vIndex - 2);
            curStroke.indices.Add(vIndex - 3);
            curStroke.indices.Add(vIndex - 4);
            curStroke.indices.Add(vIndex - 3);
            curStroke.indices.Add(vIndex - 2);
            if (useControllerAngle)
            {
                curStroke.indices.Add(vIndex - 3);
                curStroke.indices.Add(vIndex - 2);
                curStroke.indices.Add(vIndex - 1);
                curStroke.indices.Add(vIndex - 2);
                curStroke.indices.Add(vIndex - 3);
                curStroke.indices.Add(vIndex - 4);
            }

            // Update the mesh
            UpdateCurMesh();

            // Update the last point
            lastPoint = newPoint;
        }

        private Vector3 GetPerpVector(Vector3 delta, Vector3 point)
        {
            Vector3 sideDir = useControllerAngle ? GvrControllerInput.Orientation * Vector3.up : delta;
            return Vector3.Cross(sideDir, point).normalized * brushThickness;
        }

        public void Clear()
        {
            PV.RPC("ClearRPC", PhotonTargets.AllBuffered);
        }

        [PunRPC]
        private void ClearRPC()
        {
            for (int i = 0; i < strokes.Count; i++)
            {
                Destroy(strokes[i].obj);
            }
            strokes.Clear();
            System.GC.Collect();
        }
    }
}
