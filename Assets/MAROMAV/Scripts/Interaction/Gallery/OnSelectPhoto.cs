using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
	public class OnSelectPhoto : MonoBehaviour
	{
		private Button button;

		void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(PhotoToWorld);
		}

		private void PhotoToWorld()
		{
			ControllerInteractionGoogleVR.Instance.pictureGallery.OnSelectedPhoto();
		}		
	}
}

