using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace MAROMAV.CoworkSpace
{
    public class LobbyNetworkMaster : MonoBehaviour
    {
        public static readonly string VERSION = "v1.0";

        public static LobbyNetworkMaster Instance { get; private set; }

        void Init()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.ConnectUsingSettings(VERSION);
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Init();
        }

        //  재연결 시도
        public void OnReconnectPhotonServer()
        {
            PhotonNetwork.ConnectUsingSettings(VERSION);
            LobbyUIMaster.Instance.ClosePopup();
        }

        public void CreatePhotonRoom(string roomName, int maxPlayers)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)maxPlayers }, null);
        }

        public void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");
        }

        public void OnCreatedRoom()
        {
            Debug.Log("OnCreatedRoom");
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("As OnConnectedToMaster() got called, the PhotonServerSetting.AutoJoinLobby must be off. Joining lobby by calling PhotonNetwork.JoinLobby().");
            PhotonNetwork.JoinLobby();
        }

        public void OnPhotonCreateRoomFailed()
        {
            Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        }

        public void OnPhotonJoinRoomFailed(object[] cause)
        {
            Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        }

        public void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from Photon.");
            LobbyUIMaster.Instance.OnReconnectPopup();
        }

        public void OnFailedToConnectToPhoton(object parameters)
        {
            Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
        }
    }
}
