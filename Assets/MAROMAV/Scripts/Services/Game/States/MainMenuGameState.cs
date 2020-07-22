using UnityEngine;
using System.Collections;

namespace MAROMAV.CoworkSpace
{
    public class MainMenuGameState : BaseGameState
    {
        public override void InitState()
        {
            base.InitState();

            //  메인메뉴에 해당하는 플레이어를 생성한다
            GameModel.Instance.BuildPlayer();

            //  스폰포인트의 색을 모두 회색으로 만든다
            //  접속이 된 상태가 아니라는 정보 제공
            SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
            for (int i = 0; i < spawnPoints.Length; ++i)
            {
                spawnPoints[i].RestoreDefaults();
            }
        }
    }
}
