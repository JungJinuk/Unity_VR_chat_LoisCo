using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{

    public class TextKeyboard : MonoBehaviour
    {
        public Text inputText;
        // GameObject keyBoard;
        public GameObject keyboardPrefab;

        public GameObject CashedKeyboard { get; private set; }
        // Use this for initialization
        // void Start()
        // {
            // keyBoard = GameObject.Find("KeyBoardCanvas");
            // if (keyBoard == null)
            // {
            //     keyBoard = Instantiate(keyboardPrefab);
            // }
        // }

        public void ConnectKeyboard()
        {
            // keyBoard.transform.position = transform.position + Vector3.down * 1.3f + transform.forward * -1f;
            // keyBoard.transform.rotation = transform.transform.rotation;

			// if (GameModel.Instance.GetComponentInChildren<TextKeyboard>() == null)
			// {
			// 	CashedKeyboard = GameObject.Instantiate(keyboardPrefab);
            //     CashedKeyboard.transform.SetParent(GameModel.Instance.CurrentPlayer.SelectUIRoot, false);
			// }

			if (CashedKeyboard == null)
            {
				KeyBoard alreadyKeyboard = GameModel.Instance.CurrentPlayer.SelectUIRoot.GetComponentInChildren<KeyBoard>(true);
				if (alreadyKeyboard != null)
				{
					CashedKeyboard = alreadyKeyboard.gameObject;
					CashedKeyboard.SetActive(true);
					CashedKeyboard.GetComponent<KeyBoard>().selectTextInput(inputText);
				}
				else
				{
					CashedKeyboard = GameObject.Instantiate(keyboardPrefab);
					CashedKeyboard.transform.SetParent(GameModel.Instance.CurrentPlayer.SelectUIRoot, false);
					CashedKeyboard.GetComponent<KeyBoard>().selectTextInput(inputText);
				}
            }
			else
			{
				CashedKeyboard.SetActive(true);
				CashedKeyboard.GetComponent<KeyBoard>().selectTextInput(inputText);
			}
        }
    }
}
