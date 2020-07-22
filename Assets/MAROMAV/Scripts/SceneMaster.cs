using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
	public static SceneMaster Instance { get; private set; }

	void Awake()
	{
		Instance = this;
	}

	public void LoadSceneTo(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}
