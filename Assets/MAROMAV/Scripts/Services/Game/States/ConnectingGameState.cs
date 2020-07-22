using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace MAROMAV.CoworkSpace
{
    public class ConnectingGameState : BaseGameState
    {
        public override void InitState()
        {
            base.InitState();
            NetworkMaster.OnGameConnected += InitGame;
        }

        public override void FinishState()
        {
            base.FinishState();
            NetworkMaster.OnGameConnected -= InitGame;
        }

        //  photon 이 정상적으로 room에 접속했을 경우
        //  NetworkMaster에서 OnGameConnected 를 발생시킬 때 호출되는 함수
        void InitGame()
        {
            SetPlayerData();
            GameModel.Instance.ChangeGameState(new InitializingGameState());
        }

        //  접속한 photon player의 정보를 커스터마이징해서 저장한다
        void SetPlayerData()
        {
            List<int> freePositions = new List<int>();
            for(int pos = 0; pos < NetworkMaster.MAX_PLAYERS; pos++)
            {
                freePositions.Add(pos);
            }

            //  현재 접속해 있는 모든 photon player의 정보를 가지고 와서
            //  이미 저장된 player의 position에 해당하는 값을 리스트에서 지우고
            //  남은 0 ~ 5 사이의 값을 자신에게 부여한다
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                PhotonPlayer player = PhotonNetwork.playerList[i];

                if (player.CustomProperties["position"] != null)
                {
                    freePositions.Remove((int)player.CustomProperties["position"]);
                }
            }

            string playerName = string.Empty;

            switch (freePositions[0])
            {
                case 0:
                    playerName = "Player 1";
                    break;
                case 1:
                    playerName = "Player 2";
                    break;
                case 2:
                    playerName = "Player 3";
                    break;
                case 3:
                    playerName = "Player 4";
                    break;
                case 4:
                    playerName = "Player 5";
                    break;
                case 5:
                    playerName = "Player 6";
                    break;
                case 6:
                    playerName = "Player 7";
                    break;
                case 7:
                    playerName = "Player 8";
                    break;
            }

            Hashtable playerInfo = new Hashtable();
            playerInfo.Add("position", freePositions[0]);
            playerInfo.Add("name", playerName);

            //  Hashtable 로 photon player 의 커스텀 정보를 저장시켜 놓을 수 있다
            PhotonNetwork.player.SetCustomProperties(playerInfo);

            //  photon player 의 커스텀 정보를 불러오는 방법
            Debug.Log(PhotonNetwork.player.CustomProperties["position"]);
            Debug.Log(PhotonNetwork.player.ID);
        }
    }
}
