using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class LobbyUIMaster : MonoBehaviour
    {
        [SerializeField]
        GameObject noRoomInfoPanel;

        [SerializeField]
        GameObject roomPanel;

        [SerializeField]
        GameObject popupPanel;

        [SerializeField]
        GameObject createMeetingroomPopupPanel;

        [SerializeField]
        GameObject joinMeetingroomPopupPanel;

        [SerializeField]
        GameObject reconnectPopupPanel;

        [SerializeField]
        Transform listContentPanel;

		public static LobbyUIMaster Instance { get; private set; }

        public enum LobbyState
        {
            CURRENT_MEETINGS,
            PAST_MEETINGS
        }
        public static LobbyState lobbyState = LobbyState.CURRENT_MEETINGS;

		void Awake()
		{
			Instance = this;
		}

        void Start()
        {
            Init();
        }

        void Init()
        {
            lobbyState = LobbyState.CURRENT_MEETINGS;
            noRoomInfoPanel.SetActive(false);
            createMeetingroomPopupPanel.SetActive(false);
            joinMeetingroomPopupPanel.SetActive(false);
            reconnectPopupPanel.SetActive(false);
            popupPanel.SetActive(false);
            UpdateLobby();
        }

        public void ClosePopup()
        {
            createMeetingroomPopupPanel.SetActive(false);
            joinMeetingroomPopupPanel.SetActive(false);
            reconnectPopupPanel.SetActive(false);
            popupPanel.SetActive(false);
        }

        public void OnReconnectPopup()
        {
            popupPanel.SetActive(true);
            reconnectPopupPanel.SetActive(true);
        }

        public void OnCreateMeetingPopup()
        {
            popupPanel.SetActive(true);
            createMeetingroomPopupPanel.SetActive(true);
        }

        public void OnJoinMeetingPopup()
        {
            popupPanel.SetActive(true);
            joinMeetingroomPopupPanel.SetActive(true);
        }

        private void ClearRoomsPanel()
        {
            RoomProperty[] rooms = listContentPanel.GetComponentsInChildren<RoomProperty>();
            for (int i = 0; i < rooms.Length; ++i)
            {
                Destroy(rooms[i].gameObject);
            }
        }

        public void UpdateLobby()
        {
            if (lobbyState == LobbyState.CURRENT_MEETINGS)
            {
                List<MeetingRoom> meetingRooms = Entity.users[Entity.currentUserIndex].meetingRooms;
                if (meetingRooms.Count == 0)
                {
                    noRoomInfoPanel.SetActive(true);
                }
                else
                {
                    ClearRoomsPanel();
                    for (int i = 0; i < meetingRooms.Count; ++i)
                    {
                        GameObject go = GameObject.Instantiate(roomPanel);
                        go.transform.SetParent(listContentPanel, false);
                        SetMeetingRoomInfo(go, meetingRooms[i]);
                    }
                }
            }
            else
            {

            }
        }

        private void SetMeetingRoomInfo(GameObject roomPanel, MeetingRoom meetingroom)
        {
            RoomProperty roomProp = roomPanel.GetComponent<RoomProperty>();

            string meetingSize = "SMALL";

            switch (meetingroom.maxPlayers)
            {
                case 4: meetingSize = "SMALL"; break;
                case 10: meetingSize = "MEDIUM"; break;
                case 20: meetingSize = "ANNOUNCEMENT"; break;
                default: meetingSize = "SMALL"; break;
            }

            roomProp.title.text = meetingroom.roomName;
            roomProp.roomInfo.text = "CREATED TIME : " + meetingroom.createdTime + "\n"
                                    +"ROOM CODE : " + meetingroom.code + "\n"
                                    +"MEETING SIZE : " + meetingSize;
            roomProp.users.text = 0 + " / " + meetingroom.maxPlayers;
        }
    }
}
