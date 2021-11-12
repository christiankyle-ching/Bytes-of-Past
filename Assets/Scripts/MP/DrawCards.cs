using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class DrawCards : MonoBehaviour
{
    public void Draw()
    {
        PlayerManager playerManager = NetworkClient.localPlayer.GetComponent<PlayerManager>();

        string playerName = StaticData.Instance.GetPlayerName();

#if UNITY_EDITOR
        //TODO: Comment on Build (including ParallelSync)
        playerName = ClonesManager.IsClone() ?
            $"Clone {Random.Range(100, 999)}" :
            StaticData.Instance.GetPlayerName();
#endif

        playerManager.CmdReady(playerName, (int)StaticData.Instance.GetPlayerAvatar());

        gameObject.SetActive(false);
    }
}
