using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class GameModel : MonoBehaviour
    {
		#region Player Properties

		[SerializeField]
		PlayerFactory _playerFactory;
		
		#endregion

		public Dictionary<int, int> PlayersPositions { get; set; }

		//  현재 게임 상태
		//  상태시작 / 상태실행(매 프레임) / 상태종료
		BaseGameState _activeGameState;
		public BaseGameState ActiveGameState { get { return _activeGameState; } }

		//  현재 자신 플레이어
		public Player CurrentPlayer { get; set; }
	
		public static GameModel Instance { get; set; }

		void Awake()
		{
			Instance = this;
		}

		void Update()
		{
			//  현재 게임상태를 매 프레임마다 실행시킨다.
			if (_activeGameState != null)
			{
				_activeGameState.ExecuteState();
			}
		}

		//  게임상태 변경
		//  이전 게임상태 종료 >> 현재 게임상태 시작
		public void ChangeGameState(BaseGameState newState)
        {
            if(_activeGameState != null)
            {
                _activeGameState.FinishState();
            }
            _activeGameState = newState;
            _activeGameState.InitState();
        }

		//  플레이어 생성
		public void BuildPlayer()
        {
            _playerFactory.Build();
        }
    }
}
