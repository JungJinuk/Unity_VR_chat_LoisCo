using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;

namespace MAROMAV.CoworkSpace
{
    public class DummyUserData : MonoBehaviour
    {
        private static readonly string DEFAULT_ENCRYPT_KEY = "loisco";

        public static DummyUserData Instance { get; private set; }

        // [Serializable]
        // public class User
        // {
        //     public User(string email, string password, List<MeetingRoom> meetingRooms = null, UserInfo UserInfo = null)
        //     {
        //         this.email = email;
        //         this.password = password;
        //         this.meetingRooms = meetingRooms;
        //         this.UserInfo = UserInfo;
        //     }
        //     public string email;
        //     public string password;
        //     public List<MeetingRoom> meetingRooms;
        //     public UserInfo UserInfo;
        //     public int currentUserIndex;
        // }

        void Awake()
        {
            Instance = this;

            Init();
        }

        void Init()
        {
            // if (users.Count != 0)
            // {
            //     users.Clear();
            // }

            Entity.users.Clear();

            List<User> tempusers = LoadFromPrefs("UserList") as List<User>;
            for (int i = 0; i < tempusers.Count; ++i)
            {
                // users.Add(tempusers[i]);
                Entity.users.Add(new User(tempusers[i].email, tempusers[i].password, tempusers[i].meetingRooms, tempusers[i].userInfo));
            }
        }

        public void AddNewAccount(string email, string password)
        {
            // users.Add(new DummyUser(email, password));
            Entity.users.Add(new User(email, password, new List<MeetingRoom>(), new UserInfo()));
            Debug.Log(string.Format("new account created. email : {0}, password : {1}", email, password));

            SaveAsPrefs("UserList", Entity.users);
        }

        public void SaveChanges(User currentUser)
        {
            Debug.Log(Entity.currentUserIndex);
            // Entity.users[Entity.currentUserIndex] = currentUser;
            SaveAsPrefs("UserList", Entity.users);
        }

        void SaveAsPrefs(string key, object targetToSave)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, targetToSave);
            SetString(key, Convert.ToBase64String(memoryStream.GetBuffer()), DEFAULT_ENCRYPT_KEY);
        }

        object LoadFromPrefs(string key)
        {
            object targetToLoad = null;
            var data = GetString(key, DEFAULT_ENCRYPT_KEY);

            if (!string.IsNullOrEmpty(data))
            {
                var binaryFormatter = new BinaryFormatter();
                var memoryStream = new MemoryStream(Convert.FromBase64String(data));
                targetToLoad = (object)binaryFormatter.Deserialize(memoryStream);
            }
            return targetToLoad;
        }

        void SetString(string key, string value, string encryptKey)
        {
            //Hide '_key' string.
            MD5 md5Hash = MD5.Create();
            byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
            byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

            //Encrypt '_value' into a byte array
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);

            //Encrypt '_value' with 3DES
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = secret;
            des.Mode = CipherMode.ECB;
            ICryptoTransform xform = des.CreateEncryptor();
            byte[] encrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

            //Convert encrypted array into a readable string.
            string encryptedString = Convert.ToBase64String(encrypted);

            //Set the (key, encrypted value) pair in regular UserPrefs.
            PlayerPrefs.SetString(hashKey, encryptedString);
        }

        string GetString(string key, string encryptKey)
        {
            //Hide '_key' string.
            MD5 md5Hash = MD5.Create();
            byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
            byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

            // Retrieve encrypted '_value' and Base64 decode it.
            string _value = PlayerPrefs.GetString(hashKey);
            byte[] bytes = Convert.FromBase64String(_value);

            //Decrypt '_value' with 3DES
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = secret;
            des.Mode = CipherMode.ECB;
            ICryptoTransform xform = des.CreateDecryptor();
            byte[] decrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

            // decrypte_value as a proper string
            string decryptedString = System.Text.Encoding.UTF8.GetString(decrypted);
            return decryptedString;
        }
    }
}