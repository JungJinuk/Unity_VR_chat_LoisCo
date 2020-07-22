using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MAROMAV.CoworkSpace
{
    // 네트워크 상태
    public enum NetworkState
    {
        INITIALIZING, CONNECTING_TO_SERVER, CREATING_ROOM, ROOM_CREATED, JOINING_ROOM,
        ROOM_JOINED, PLAYING, SOME_PLAYER_CONNECTED, SOME_PLAYER_DISCONNECTED, DISCONNECTED, MASTERCLIENT_CHANGED
    }

    public class NetworkMaster : MonoBehaviour
    {
        //  게임 버전 : 동일한 앱과 동일한 버전을 가지고 있는 사람들끼리 Room에 입장할 수 있다
        public const string NETCODE_VERSION = "v1.0";

        //  룸에 최대로 입장 할 수 있는 플레이어 수
        public const int MAX_PLAYERS = 8;

        //  현재 네트워크 연결 상태
        public NetworkState ActiveState { get; private set; }

        private Connection _connection;

        //  photon server 연결시에 네트워크 상태 변경 이벤트 액션 (인자는 NetworkState)
        public static event Action<NetworkState> OnNetworkStateChange;

        //  photon server 가 정상적으로 room에 접속 했을 경우에 OnGameConneted 액션 실행
        public static event Action OnGameConnected;
        public static event Action<PhotonPlayer> OnSomePlayerConnected;
        public static event Action<PhotonPlayer> OnSomePlayerDisconnected;

        //  master client 가 변경되었을 경우 액션 실행
        public static event Action<PhotonPlayer> OnMasterClientChanged;

        public static NetworkMaster Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            _connection = GetComponent<Connection>();
        }

        //  멀티플레이용 룸에 입장 시작시에 호출
        public void StartMultiplayerGame()
        {
            _connection.Init();
            _connection.Connect();
        }

        //  멀티플레이 종료 시에 호출
        public void EndMultiplayerGame()
        {
            _connection.Disconnect();
        }

        //  photon server 로 부터 네트워크 상태를 받아서 현재 네트워크의 상태를 변경한다
        public void ChangeNetworkState(NetworkState newState, object stateData = null)
        {
            ActiveState = newState;

            if (OnNetworkStateChange != null)
            {
                OnNetworkStateChange(ActiveState);
            }

            switch (ActiveState)
            {
                //  룸을 만들고 접속했을 경우
                case NetworkState.ROOM_CREATED:
                    if (OnGameConnected != null)
                    {
                        OnGameConnected();
                    }
                    ChangeNetworkState(NetworkState.PLAYING);
                    break;
                //  이미 룸이 있어서 해당 룸에 접속했을 경우
                case NetworkState.ROOM_JOINED:
                    if (OnGameConnected != null)
                    {
                        OnGameConnected();
                    }
                    // NotifyToUpdateScore();
                    ChangeNetworkState(NetworkState.PLAYING);
                    break;
                //  다른 플레이어가 같은 룸에 접속했을 경우
                case NetworkState.SOME_PLAYER_CONNECTED:
                    if (OnSomePlayerConnected != null)
                    {
                        OnSomePlayerConnected((PhotonPlayer)stateData);
                    }
                    ChangeNetworkState(NetworkState.PLAYING);
                    break;
                //  다른 플레이어가 같은 룸에 있다가 접속이 끊겼을 경우
                case NetworkState.SOME_PLAYER_DISCONNECTED:
                    if (OnSomePlayerDisconnected != null)
                    {
                        OnSomePlayerDisconnected((PhotonPlayer)stateData);
                    }
                    // NotifyToUpdateScore();
                    ChangeNetworkState(NetworkState.PLAYING);
                    break;
                //  master client 가 변경되었을 경우
                case NetworkState.MASTERCLIENT_CHANGED:
                    if (OnMasterClientChanged != null)
                    {
                        OnMasterClientChanged((PhotonPlayer)stateData);
                    }
                    // NotifyToUpdateScore();
                    ChangeNetworkState(NetworkState.PLAYING);
                    break;
                case NetworkState.DISCONNECTED:
                    {
                        GameView.Instance.ShowConnectionRetryPanel();
                    }
                    // NotifyToUpdateScore();
                    // ChangeNetworkState(NetworkState.PLAYING);
                    break;
            }
        }
    }
}