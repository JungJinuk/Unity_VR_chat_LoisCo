using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MAROMAV.CoworkSpace;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnPoint : MonoBehaviour
{
	[SerializeField]
    private int spawnpointId;

    private MeshRenderer[] meshRenderers;


    public void Awake()
	{
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material.color = Color.grey;
        }

	}

    //  접속한 player 의 속성이 바뀌었을 경우에 호출되는 함수
    //  Connecting Game State 에서 player 의 커스텀 속성을 변경했을 경우에 호출된다
	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        Hashtable props = (Hashtable)playerAndUpdatedProps[1];

        if (!props.ContainsKey("position"))
        {
            return;
        }

        int position = (int)props["position"];

        if (position != spawnpointId)
        {
            return;
        }

        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material.color = PlayerFactory.Instance.GetColor(spawnpointId);
        }

    }

    //  photon player 의 연결이 끊겼을 경우 내부적으로 호출되는 함수 (연결이 끊긴 player)
    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        if (!player.CustomProperties.ContainsKey("position"))
        {
            return;
        }

        int position = (int)player.CustomProperties["position"];

        if (position != spawnpointId)
        {
            return;
        }

        //  연결이 끊긴 플레이어의 자리를 기본색으로 바꾼다
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material.color = Color.grey;
        }
    }

    //  플레이어의 스폰 위치의 자리색을 기본값으로
    public void RestoreDefaults()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material.color = Color.grey;
        }
    }
}
