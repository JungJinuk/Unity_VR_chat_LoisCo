using UnityEngine;
using System.Collections;

namespace MAROMAV.CoworkSpace
{
    public class InitializingGameState : BaseGameState
    {
        public override void InitState()
        {
            base.InitState();

            //  멀티플레이에 해당하는 플레이어 생성
            GameModel.Instance.BuildPlayer();
            SpawnUI();

            GameModel.Instance.ChangeGameState(new PlayingGameState());
        }

        void SpawnUI()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            GameView.Instance.ShowWelcomePanel();
#else
            //  앱에 대한 권한을 부여 받지 않았을 경우 권한 승인 패널을 띄운다
            if (!GamePermissions.Instance.IsRecodePermissionAccepted)
            {
                GameView.Instance.ShowPermissionRequestPanel();
            }
            else  //  그렇지 않다면 환영 패널을 띄운다
            {
                GameView.Instance.ShowWelcomePanel();
            }
#endif
        }
    }
}
