using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace MAROMAV.CoworkSpace
{
    public class GameMaster : MonoBehaviour
    {
		public static GameMaster Instance { get; private set; }

        [SerializeField]
        private float vrRenderScale = 0.7f;

		void Awake()
		{
			Instance = this;
            VRSettings.renderScale = vrRenderScale;
            Debug.Log("VR render scale is : " + VRSettings.renderScale);
		}

        void Start()
        {
            //  메인메뉴를 실행한다.
            InitMainMenu();
        }

        public void InitMainMenu()
        {
            //  게임상태를 메인메뉴게임상태로 변경하고 실행한다.
            GameModel.Instance.ChangeGameState(new MainMenuGameState());

            //  메인메뉴패널을 보여준다.
            GameView.Instance.ShowMainMenuPanel();
        }

        //  Cowork space에 입장을 시도한다.
        public void StartMultiplayerGame()
        {
            //  게임상태를 연결중 상태로 변경하고 실행한다.
            GameModel.Instance.ChangeGameState(new ConnectingGameState());

            //  네트워크 연결상태를 보여준다.
            GameView.Instance.ShowNetworkPanel();

            //  photon 서버에 연결을 시도한다.
            NetworkMaster.Instance.StartMultiplayerGame();
        }
    }
}