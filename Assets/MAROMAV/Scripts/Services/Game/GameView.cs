using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class GameView : MonoBehaviour
    {
        [Header("Presenter Panels")]

        [SerializeField]
        GameObject _networkPanel;

        [SerializeField]
        GameObject _mainMenuPanel;

        [SerializeField]
        GameObject _gamePopupPanel;

        [SerializeField]
        GameObject _permissionRequestPanel;

        [SerializeField]
        GameObject _welcomePanel;
        
        [SerializeField]
        GameObject _connectionRetryPanel;

        public Transform UIRoot { get; set; }

        public static GameView Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 룸에 접속하기 전 메인화면 패널을 띄운다
        /// </summary>
		public void ShowMainMenuPanel()
        {
            if (UIRoot == null) return;

            ClearUIRoot();

            var panel = Instantiate(_mainMenuPanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);
        }

        /// <summary>
        /// 룸에 접속중일 때 Photon 네트워크 상태 표현 패널을 띄운다
        /// </summary>
		public void ShowNetworkPanel()
        {
            if (UIRoot == null) return;

            ClearUIRoot();
            var panel = Instantiate(_networkPanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);
        }

        /// <summary>
        /// Show popup windows during gameplay
        /// </summary>
        /// <param name="message">main message</param>
        /// <param name="subMessage">optional secondary message</param>
        /// <param name="selfDestroyTime">time after which windows will be disposed automatically</param>
        public void ShowGamePopupPanel(string message, string subMessage = "", float selfDestroyTime = 2f)
        {
            if (UIRoot == null) return;

            ClearUIRoot();

            var panel = Instantiate(_gamePopupPanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);

            var presenter = panel.GetComponent<GamePopupPresenter>();
            presenter.UpdateMessage(message, subMessage, selfDestroyTime);
        }

        /// <summary>
        /// 정상적으로 룸에 접속했을 경우 환영 팝업창을 띄운다
        /// </summary>
        /// <param name="selfDestroyTime">창이 뜬 뒤 자동으로 사라지는 시간 (기본3초)</param>
        public void ShowWelcomePanel(float selfDestroyTime = 3f)
        {
            if (UIRoot == null) return;

            ClearUIRoot();
            var panel = Instantiate(_welcomePanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);

            Destroy(panel, selfDestroyTime);
        }

        /// <summary>
        /// 앱에 대한 권한이 부여되지 않았을 경우에 권한 승인하기 패널을 띄운다
        /// </summary>
        public void ShowPermissionRequestPanel()
        {
            if (UIRoot == null) return;

            ClearUIRoot();
            var panel = Instantiate(_permissionRequestPanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);
        }

        /// <summary>
        /// 서버에 연결 실패했거나 네트워크 상태가 좋지 않아 룸에 입장하지 못했을 경우 다시 연결 패널을 띄운다
        /// </summary>
        /// <param name="message">server state message</param>
        public void ShowConnectionRetryPanel()
        {
            if (UIRoot == null) return;

            ClearUIRoot();
            var panel = Instantiate(_connectionRetryPanel, UIRoot.position, Quaternion.identity) as GameObject;
            panel.transform.SetParent(UIRoot, false);

            var presenter = panel.GetComponent<ConnectionRetryPresenter>();
            presenter.UpdateMessage();
        }

        //  UIRoot에 있는 기존의 패널을 모두 삭제한다
        private void ClearUIRoot()
        {
            foreach (Transform t in UIRoot)
            {
                Destroy(t.gameObject);
            }
        }
    }
}
