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

        string playerName = PlayerPrefs.GetString("Profile_Name", "");

#if UNITY_EDITOR
        //TODO: Comment on Build (including ParallelSync)
        playerName = ClonesManager.IsClone() ?
            $"Clone {Random.Range(100, 999)}" :
            PlayerPrefs.GetString("Profile_Name", "");
#endif

        playerManager.CmdReady(playerName);

        gameObject.SetActive(false);
    }
}
