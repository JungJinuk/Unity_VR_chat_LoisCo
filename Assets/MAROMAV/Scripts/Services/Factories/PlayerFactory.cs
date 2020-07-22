using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace MAROMAV.CoworkSpace
{
    public class PlayerFactory : MonoBehaviour
    {
        [SerializeField]
        bool useNonVrPlayerInEditor;

        //  아바타가 다른 데이드림용 플레이어 프리팹 리스트
        // [SerializeField]
        public GameObject[] googleVrDaydreamPlayerPrefabs;

        //  아바타가 다른 카드보드용 플레이어 프리팹 리스트
        public GameObject[] googleVrCardboardPlayerPrefabs;

        [SerializeField]
        GameObject _googleVrPlayerForLobyPrefab;

        //  여러 플레이어들의 색 리스트
        public Color[] playerColors;

        //  여러 플레이어들의 색 매터리얼 리스트
        [SerializeField]
        public Material[] playerColorMaterials;

        //  스폰 포인트
        [SerializeField]
        Transform _playerSpawnPoints;

        //  실제 플레이에 사용될 플레이어 프리팹
        //  실행 플랫폼에 따라 플레이어 프리팹을 다르게 설정한다
        private GameObject[] _playerPrefabs;

        //  실행 플랫폼에 따라 로비에 해당하는 플레이어를 다르게 설정한다
        private GameObject _playerForLobyPrefab;

        public static PlayerFactory Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            useNonVrPlayerInEditor = false;

            //  앱을 실행하는 플랫폼에 따라 다른 플레이어 프리팹을 사용한다
#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
            // _playerPrefabs = googleVrPlayerPrefabs;
            // _playerForLobyPrefab = _googleVrPlayerForLobyPrefab;
#endif
        }

        public Transform PlayerSpawnPoints
        {
            get
            {
                return _playerSpawnPoints;
            }
        }

        //  현재 게임 상태에 따라 다른 플레이어를 빌드한다
        public void Build()
        {
            if (GameModel.Instance.ActiveGameState is InitializingGameState)
            {
                BuildPlayerForGame();
            }
            else if (GameModel.Instance.ActiveGameState is MainMenuGameState)
            {
                BuildPlayerForMenu();
            }
        }

        //  메인 스페이스 입장했을 때 플레이어 생성
        public void BuildPlayerForGame()
        {
            if (GameModel.Instance.CurrentPlayer != null)
            {
                GameObject.DestroyImmediate(GameModel.Instance.CurrentPlayer.gameObject);
            }

            //  photon 네트워크에 접속하면 ConnectingGameState 에서 photon player에 대한 커스텀 속성을 설정한다
            //  커스텀 속성이 완료 된 상태이므로 다음과 같이 사용한다
            int positionIndex = (int)PhotonNetwork.player.CustomProperties["position"];
            Vector3 spawnPoint = PlayerSpawnPoints.GetChild(positionIndex).position;
            Quaternion spawnRotation = PlayerSpawnPoints.GetChild(positionIndex).rotation;
            spawnPoint.y += 1.75f;

            bool isDaydream = GameObject.FindObjectOfType<GooglePlatformMaster>().IsDaydream;

            if (isDaydream)
            {
                // 플리이어 프리팹을 photon network 로 생성해서 같은 룸에 접속해 있는 모두에게 추적되도록 설정한다
                GameObject go = PhotonNetwork.Instantiate(googleVrDaydreamPlayerPrefabs[positionIndex].name, spawnPoint, spawnRotation, 0) as GameObject;
                GameModel.Instance.CurrentPlayer = go.GetComponent<Player>();
            }
            else
            {
                GameObject go = PhotonNetwork.Instantiate(googleVrCardboardPlayerPrefabs[positionIndex].name, spawnPoint, spawnRotation, 0) as GameObject;
                GameModel.Instance.CurrentPlayer = go.GetComponent<Player>();
            }

            if (!useNonVrPlayerInEditor)
            {
                Player currentPlayer = GameModel.Instance.CurrentPlayer;
                currentPlayer.CameraRig.SetActive(true);
                currentPlayer.UIRoot.gameObject.SetActive(true);
                currentPlayer.NameUIRoot.gameObject.SetActive(true);
                currentPlayer.Head.SetActive(true);

                if (isDaydream)
                {
                    currentPlayer.ClickMenuUIRoot.SetActive(true);
                    currentPlayer.SelectUIRoot.gameObject.SetActive(true);
                }

                GameView.Instance.UIRoot = GameModel.Instance.CurrentPlayer.UIRoot;
            }
        }

        //  로비에 해당하는 플레이어 생성
        public void BuildPlayerForMenu()
        {
            if (GameModel.Instance.CurrentPlayer != null)
            {
                PhotonNetwork.Destroy(GameModel.Instance.CurrentPlayer.gameObject);
            }

            Vector3 spawnPoint = PlayerSpawnPoints.GetChild(0).position;
            spawnPoint.y += 1.75f;

            // if (GameObject.FindObjectOfType<GooglePlatformMaster>().IsDaydream)
            // {
            //     //  플레이어 프리팹 생성을 로컬로 생성한다
            //     GameObject go = GameObject.Instantiate(_googleVrPlayerForLobyPrefab[0], spawnPoint, Quaternion.identity) as GameObject;
            //     GameModel.Instance.CurrentPlayer = go.GetComponent<Player>();
            // }
            // else
            // {
            //     //  카드보드용 플레이어 프리팹 생성을 로컬로 생성한다
            //     GameObject go = GameObject.Instantiate(_googleVrPlayerForLobyPrefab[1], spawnPoint, Quaternion.identity) as GameObject;
            //     GameModel.Instance.CurrentPlayer = go.GetComponent<Player>();
            // }

            GameObject go = GameObject.Instantiate(_googleVrPlayerForLobyPrefab, spawnPoint, Quaternion.identity) as GameObject;
            GameModel.Instance.CurrentPlayer = go.GetComponent<Player>();

            Player currentPlayer = GameModel.Instance.CurrentPlayer;

            bool isDaydream = GameObject.FindObjectOfType<GooglePlatformMaster>().IsDaydream;

            //Initializing UI
            if (!useNonVrPlayerInEditor)
            {
                currentPlayer.CameraRig.SetActive(true);
                currentPlayer.UIRoot.gameObject.SetActive(true);
                GameView.Instance.UIRoot = GameModel.Instance.CurrentPlayer.UIRoot;

                if (isDaydream)
                {
                    currentPlayer.ControllerMain.SetActive(true);
                    currentPlayer.ControllerPointer.SetActive(true);
                    currentPlayer.ReticlePointer.SetActive(false);
                }
                else
                {
                    currentPlayer.ControllerMain.SetActive(false);
                    currentPlayer.ControllerPointer.SetActive(false);
                    currentPlayer.ReticlePointer.SetActive(true);
                }
            }
        }

        //  스폰지역에 해당하는 오브젝트 색 변경시에 사용
        public Color GetColor(int position)
        {
            switch (position)
            {
                case 0: return playerColors[0];
                case 1: return playerColors[1];
                case 2: return playerColors[2];
                case 3: return playerColors[3];
                case 4: return playerColors[4];
                case 5: return playerColors[5];
                case 6: return playerColors[6];
                case 7: return playerColors[7];
                case 8: return Color.grey;
                default: return Color.grey;
            }
        }

        //  플레이어 색 매터리얼 사용
        public Material GetMaterialByPlayer(int index)
        {
            switch (index)
            {
                case 0: return playerColorMaterials[0];
                case 1: return playerColorMaterials[1];
                case 2: return playerColorMaterials[2];
                case 3: return playerColorMaterials[3];
                case 4: return playerColorMaterials[4];
                case 5: return playerColorMaterials[5];
                case 6: return playerColorMaterials[6];
                case 7: return playerColorMaterials[7];
                case 8: return playerColorMaterials[8];
                default: return playerColorMaterials[8];
            }
        }
    }
}
