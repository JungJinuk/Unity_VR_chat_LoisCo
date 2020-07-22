using UnityEngine;
using System.Collections;

namespace MAROMAV.CoworkSpace
{
    public class PlayingGameState : BaseGameState
    {
        // private bool isActiveMasterClientObj = false;

        public override void InitState()
        {
            base.InitState();

            NetworkMaster.OnSomePlayerConnected += SomeoneConnected;
            NetworkMaster.OnSomePlayerDisconnected += SomeoneDisconnected;
        }

        public override void FinishState()
        {
            base.FinishState();

            NetworkMaster.OnSomePlayerConnected -= SomeoneConnected;
            NetworkMaster.OnSomePlayerDisconnected -= SomeoneDisconnected;
        }

        public override void ExecuteState()
        {
            base.ExecuteState();
        }

        //  connection 에서 같은 room 에 다른 photon player가 접속했을 경우 photon 내부에서 콜백된 함수에 의해
        //  NetworkMaster 에서 OnSomePlayerConnect(PhotonPlayer) 를 발생시켰을 경우
        //  호출되는 함수
        void SomeoneConnected(PhotonPlayer somePlayer)
        {
            GameView.Instance.ShowGamePopupPanel("NEW PLAYER", "Player ID : " + somePlayer.ID + " connected");
            GameSound.Instance.PlaySomeoneJoinSound();
        }

        //  connection 에서 같은 room 에 있던 다른 photon player의 접속이 끊겼을 경우 photon 내부에서 콜백된 함수에 의해
        //  NetworkMaster 에서 OnSomePlayerDisconnect(PhotonPlayer) 를 발생시켰을 경우
        //  호출되는 함수
        void SomeoneDisconnected(PhotonPlayer somePlayer)
        {
            GameView.Instance.ShowGamePopupPanel("PLAYER DISCONNECTED", (string)somePlayer.CustomProperties["name"] + " disconnected");
            GameSound.Instance.PlaySomeoneLeaveSound();
        }
    }
}
