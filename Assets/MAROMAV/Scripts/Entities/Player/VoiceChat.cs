using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class VoiceChat : Photon.MonoBehaviour
    {
        PhotonVoiceRecorder rec;

        void Awake()
        {
			// if (GameModel.Instance.ActiveGameState is MainMenuGameState)
			// {
			// 	return;
			// }

            if (photonView.isMine)
            {
                rec = GetComponent<PhotonVoiceRecorder>();
				rec.enabled = true;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}