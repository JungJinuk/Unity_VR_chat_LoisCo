using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    //  플레이어의 이름 UI를 생성한다
    public class PlayerName : MonoBehaviour
    {
        [SerializeField]
        GameObject _playerNamePrefab;

        GameObject _playerNameUI;

        void Start()
        {
            _playerNameUI = Instantiate(_playerNamePrefab) as GameObject;

            Player player = this.GetComponent<Player>();
            _playerNameUI.transform.SetParent(player.NameUIRoot, false);
            _playerNameUI.GetComponent<PlayerNamePresenter>().SetTarget(player);
        }
    }
}

