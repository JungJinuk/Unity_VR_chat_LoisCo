using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
	[RequireComponent(typeof(PhotonView))]
    public class Controller : MonoBehaviour
    {
        protected virtual void Awake()
        {
			if (GameModel.Instance.ActiveGameState is MainMenuGameState)
			{
				return;
			}
			
            var photonView = GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                Destroy(this.GetComponent<GvrTrackedController>());
            }
        }
    }
}