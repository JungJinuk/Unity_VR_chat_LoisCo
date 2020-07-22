using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class ConnectionRetryPresenter : MonoBehaviour
    {
		[SerializeField]
        Text _message;

		public void OnConnectionRetryButtonClick()
        {
            GameMaster.Instance.StartMultiplayerGame();
        }

        public void UpdateMessage()
        {
            if (_message != null)
            {
                _message.text = string.Format("Not connected. Please check network.\nconnection state : {0}",
                                            PhotonNetwork.connectionStateDetailed);
            }
        }
    }
}
