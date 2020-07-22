using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class NetworkPresenter : MonoBehaviour
    {
        [SerializeField]
        Text _message;

        void OnEnable()
        {
            NetworkMaster.OnNetworkStateChange += UpdateMessage;
        }

        void OnDisable()
        {
            NetworkMaster.OnNetworkStateChange -= UpdateMessage;
        }

        //  NetworkMaster 의 OnNetworkStateChange(NetworkState) 가 발생할 때 마다
        //  호출되는 함수
        void UpdateMessage(NetworkState state)
        {
            switch (state)
            {
                case NetworkState.INITIALIZING:
                    _message.text = "Initializing";
                    break;
                case NetworkState.CONNECTING_TO_SERVER:
                    _message.text = "Connecting to server";
                    break;
                case NetworkState.JOINING_ROOM:
                    _message.text = "Joining Room";
                    break;
                case NetworkState.CREATING_ROOM:
                    _message.text = "Creating room";
                    break;
                case NetworkState.PLAYING:
                    _message.text = "Playing";
                    break;
                default:
                    break;
            }
        }
    }
}
