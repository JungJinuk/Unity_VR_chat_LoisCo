using UnityEngine;
using System.Collections;

namespace MAROMAV.CoworkSpace
{
    public class MainMenuPresenter : MonoBehaviour
    {
        public void OnStartButtonClick()
        {
            GameMaster.Instance.StartMultiplayerGame();
        }
    }
}
