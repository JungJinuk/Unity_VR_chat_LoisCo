using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MAROMAV.CoworkSpace
{
    [Serializable]
    public static class Entity
    {
        public static List<User> users = new List<User>();
        public static List<MeetingRoom> meetingRooms = new List<MeetingRoom>();
        public static int currentUserIndex;
    }

    [Serializable]
    public class User
    {
        public User(string email, string password, List<MeetingRoom> meetingRooms, UserInfo userInfo)
        {
            this.email = email;
            this.password = password;
            this.meetingRooms = meetingRooms;
            this.userInfo = userInfo;
        }
        public string email;
        public string password;
        public List<MeetingRoom> meetingRooms;
        public UserInfo userInfo;
        // public int currentUserIndex;
    }

    [Serializable]
    public class MeetingRoom
    {
        public MeetingRoom(string roomName, int maxPlayers, string createdTime,
                            string endedTime, string code, string owner)
        {
            this.roomName = roomName;
            this.maxPlayers = maxPlayers;
            this.createdTime = createdTime;
            this.endedTime = endedTime;
            this.code = code;
            this.owner = owner;
        }

        public string roomName;
        public int maxPlayers;
        public string createdTime;
        public string endedTime;
        public string code;
        public string owner;
    }

    [Serializable]
    public class UserInfo
    {
        public UserInfo(string nickName = "", bool isMan = true, string nation = "",
                        string companyName = "", string companyIndustry = "", int companySize = 0)
        {
            this.nickName = nickName;
            this.isMan = isMan;
            this.nation = nation;
            this.companyName = companyName;
            this.companyIndustry = companyIndustry;
            this.companySize = companySize;
        }

        public string nickName;
        public bool isMan;
        public string nation;
        public string companyName;
        public string companyIndustry;
        public int companySize;
    }
}