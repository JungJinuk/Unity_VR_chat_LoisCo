using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MAROMAV.CoworkSpace
{
    public class CreateMeetingRoomPopup : MonoBehaviour
    {
        [SerializeField]
        Button createButton;

        [SerializeField]
        InputField roomName;

        [SerializeField]
        Dropdown roomState;

        void Awake()
        {
            createButton.onClick.AddListener(OnCreateMeetingRoom);
        }

        private MeetingRoom SetMeetingRoomInfo()
        {
            int state = roomState.value;
            int maxPlayers = 4;
            string name = roomName.text;
            string createdTime = DateTime.Now.ToString("yyyy. MM. dd. hh:mm tt");
            string endedTime = string.Empty;
            string code = string.Empty;
            string owner = "admin";

            if (string.IsNullOrEmpty(name))
            {
                //  popup 이름 입력해주세요
                return null;
            }

            //  현재 로비에 존재하는 room name 중복체크
            //  popup 존재하는 room name 입니다.
            //  return null;

            //  room state 에 따라 max player 결정
			switch (state)
            {
                case 0: maxPlayers = 4; break;
                case 1: maxPlayers = 10; break;
                case 2: maxPlayers = 20; break;
                default: maxPlayers = 4; break;
            }

            for (int i = 0; i < 6; ++i)
            {
                code += UnityEngine.Random.Range(0, 9);
            }

            return new MeetingRoom(name, maxPlayers, createdTime, endedTime, code, owner);
        }

        private void OnCreateMeetingRoom()
        {
            MeetingRoom meetingRoom = SetMeetingRoomInfo();
            if (meetingRoom != null)
            {
                Entity.users[Entity.currentUserIndex].meetingRooms.Insert(0, meetingRoom);
                DummyUserData.Instance.SaveChanges(Entity.users[Entity.currentUserIndex]);
                LobbyUIMaster.Instance.ClosePopup();
                LobbyUIMaster.Instance.UpdateLobby();
            }
			// LobbyNetworkMaster.Instance.CreateMeetingRoom(name, maxPlayers);
        }
    }
}
