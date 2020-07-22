using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
	public class PlayerNamePresenter : Photon.PunBehaviour
	{
		[SerializeField]
		Text _plyaerNameText;

		[SerializeField]
		Image _playerColorSprite;

		[SerializeField]
		Image _speakerSprite;

		[SerializeField]
		Image _masterClientSprite;

		private PhotonVoiceSpeaker _photonSpeaker;

		private Player _target;

		//  플레이어의 이름과 색을 지정한다
		public void SetTarget(Player target)
		{
			_target = target;

			if (_target == null)
			{
				Debug.Log("Missing PlayerName target", this);
				return;
			}

			//  플레이어의 이름
			_plyaerNameText.text = (string)_target.MyPhotonView.owner.CustomProperties["name"];

			//  플레이어의 색
			int playerIndex = (int)_target.MyPhotonView.owner.CustomProperties["position"];
			_playerColorSprite.color = PlayerFactory.Instance.GetColor(playerIndex);

			//  방장 표시
			if (_target.MyPhotonView.owner.IsMasterClient)
			{
				_masterClientSprite.enabled = true;
			}

			if (_photonSpeaker == null)
			{
				_photonSpeaker = _target.GetComponent<PhotonVoiceSpeaker>();
			}

			//  현재 플레이어에게 잘보이도록 표시
			if (!_target.MyPhotonView.isMine)
			{
				Transform currentPlayer = GameModel.Instance.CurrentPlayer.CameraRig.transform;
				transform.LookAt(currentPlayer);
			}
		}

		void OnEnable()
		{
			NetworkMaster.OnMasterClientChanged += OnChangedMasterClient;
		}

		void OnDisable()
		{
			NetworkMaster.OnMasterClientChanged -= OnChangedMasterClient;
		}

		void OnChangedMasterClient(PhotonPlayer newMasterClient)
        {
			_masterClientSprite.enabled = false;

			if (newMasterClient == _target.MyPhotonView.owner)
			{
				_masterClientSprite.enabled = true;
			}
        }

		void Update()
		{
			//  자신에게는 스피커 UI가 보이지 않는다
			if (_photonSpeaker.photonView.isMine)
			{
				return;
			}

			//  말하는 플레이어의 스피커가 음성 감지보다 높을 경우 UI를 켠다
			_speakerSprite.enabled = _photonSpeaker.IsPlaying;
		}
	}
}
