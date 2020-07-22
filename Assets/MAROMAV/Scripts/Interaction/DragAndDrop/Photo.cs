using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class Photo : Photon.PunBehaviour
    {
        public Material basicEdgeColor;
        public Transform edge;

        public bool IsCatched
        {
            get
            {
                return isCatched;
            }
            set
            {
                isCatched = value;
            }
        }
        private bool isCatched = false;

        public int AssignedColorForUserId
        {
            get
            {
                return _assignedColorForUserId;
            }
        }
        private int _assignedColorForUserId;

        private MeshRenderer[] _edgeRenderers;

        void Start()
        {
            _edgeRenderers = edge.GetComponentsInChildren<MeshRenderer>();
            ChangeEdges(basicEdgeColor);
        }

        void Update()
        {
            if (isCatched)
            {
                if (photonView.ownerId != _assignedColorForUserId)
                {
                    OnChangeColor();
                    _assignedColorForUserId = photonView.ownerId;
                }
            }
        }

        private void ChangeEdges(Material color)
        {
            for (int i = 0; i < _edgeRenderers.Length; i++)
            {
                _edgeRenderers[i].material = color;
            }
        }

        public void OnSelectPhoto(byte[] fileData)
        {
            photonView.RPC("OnSelectPhotoRPC", PhotonTargets.AllBuffered, fileData);
        }

        [PunRPC]
        public void OnSelectPhotoRPC(byte[] fileData)
        {
            Texture2D new_texture = new Texture2D(16, 16);
            new_texture.LoadImage(fileData);
            int size = Mathf.Min(new_texture.width, new_texture.height);
            transform.localScale = new Vector3(new_texture.width / size, new_texture.height / size, 1f);
            transform.GetComponent<MeshRenderer>().material.mainTexture = new_texture;
        }

        public void OnChangeColor()
        {
            int index = (int)photonView.owner.CustomProperties["position"];
            photonView.RPC("OnChangeColorRPC", PhotonTargets.All, index);
        }

        [PunRPC]
        private void OnChangeColorRPC(int ownerPositionId)
        {
            ChangeEdges(PlayerFactory.Instance.GetMaterialByPlayer(ownerPositionId));
        }

        public void OnChangeColorBasic()
        {
            photonView.RPC("OnChangeColorBasicRPC", PhotonTargets.All);
        }

        //  선택한 2D 오브젝트의 엣지를 모두 기본색으로 바꾼다
        [PunRPC]
        private void OnChangeColorBasicRPC()
        {
            ChangeEdges(basicEdgeColor);
        }
    }
}
