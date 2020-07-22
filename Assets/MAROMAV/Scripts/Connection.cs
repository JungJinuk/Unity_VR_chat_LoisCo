using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace MAROMAV.CoworkSpace
{
    public class Connection : PunBehaviour
    {
        public void Init()
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.INITIALIZING);

            //  로비에 접속하는 유무
            //  랜덤으로 아무 room 에 접속하게 하려면 false로 한다
            PhotonNetwork.autoJoinLobby = false;
        }

        public void Connect()
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.CONNECTING_TO_SERVER);

            //  photon network 가 연결되어 있다면 Master 에 연결한다
            if (PhotonNetwork.connected)
            {
                OnConnectedToMaster();
            }
            else
            {
                //  photon network 가 연결되어 있지 않다면 새로 Master 서버를 만들고 연결한다
                PhotonNetwork.ConnectUsingSettings(NetworkMaster.NETCODE_VERSION);
            }
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        #region PUN Callbacks
        //  photon client 를 총괄하는 Master에 접속하면 호출되는 함수
        public override void OnConnectedToMaster()
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.JOINING_ROOM);

            //  photon이 room 에 접속중이라면 해당 room에 접속한다
            if (PhotonNetwork.inRoom)
            {
                OnJoinedRoom();
            }
            else
            {
                NetworkMaster.Instance.ChangeNetworkState(NetworkState.JOINING_ROOM);
                //  room 에 접속중이 아니라면 열려있는 room을 찾아서 해당 room에 접속한다
                //  room 접속에 실패하면 OnPhotonRandomJoinFailed 가 호출된다
                PhotonNetwork.JoinRandomRoom();
            }
        }

        //  photon 에 존재하는 room 이 없어서 room에 접속이 실패했을 경우 호출되는 함수
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.CREATING_ROOM);

            //  photon 에서 room 을 만드는 함수
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = NetworkMaster.MAX_PLAYERS }, null);
        }

        //  photon 에 이미 room이 존재하여 해당 room 에 접속하면 호출되는 함수
        public override void OnJoinedRoom()
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.ROOM_JOINED);
        }

        //  photon 에서 room 을 생성하고 접속하면 호출되는 함수
        public override void OnCreatedRoom()
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.ROOM_CREATED);
        }

        //  photon 에서 room 을 나갔을 때 호출되는 함수
        public override void OnLeftRoom()
        {
            string[] toRemoveProperties = {"name", "position"};
            PhotonNetwork.RemovePlayerCustomProperties(toRemoveProperties);
            GameObject.DestroyImmediate(GameModel.Instance.CurrentPlayer.gameObject);
            GameMaster.Instance.InitMainMenu();
        }

        public void OnFailedToConnectToPhoton(object parameters)
        {
            Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
        }

        // photon room 에 있는 local 플레이어가 접속이 끊어지면 호출되는 함수
        public override void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from Photon.");
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.DISCONNECTED);
        }

        /// <summary>
        /// photon room 에 다른 플레이어가 접속했을 때 호출되는 함수
        /// </summary>
        /// <param name="newPlayer">접속한 PhotonPlayer (photon 내부에서 던저지는 인자)</param>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.SOME_PLAYER_CONNECTED, newPlayer);
        }

        /// <summary>
        /// photon room 에 있던 다른 플레이어가 접속이 끊어지면 호출되는 함수
        /// </summary>
        /// <param name="newPlayer">접속이 끊긴 PhotonPlayer (photon 내부에서 던저지는 인자)</param>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.SOME_PLAYER_DISCONNECTED, otherPlayer);
        }

        /// <summary>
        /// master client 가 변경되었을 경우에 호출되는 함수
        /// </summary>
        /// <param name="newMasterClient">새롭게 master client 를 부여받은 PhotonPlayer</param>
        public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {
            NetworkMaster.Instance.ChangeNetworkState(NetworkState.MASTERCLIENT_CHANGED, newMasterClient);
        }

        #endregion 
    }
}