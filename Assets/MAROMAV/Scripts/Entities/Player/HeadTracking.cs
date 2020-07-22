using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
	[RequireComponent(typeof(PhotonView))]
    public class HeadTracking : MonoBehaviour
    {
		protected virtual void Awake()
		{
			var photonView = GetComponent<PhotonView>();
			if (!photonView.isMine)
			{
				Destroy(this);
			}
		}
    }
}

