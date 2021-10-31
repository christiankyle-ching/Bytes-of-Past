using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
//using ParrelSync;

public class DrawCards : MonoBehaviour
{
    public void Draw()
    {
        PlayerManager playerManager = NetworkClient.localPlayer.GetComponent<PlayerManager>();

        string playerName = PlayerPrefs.GetString("Profile_Name", "");

        //TODO: Comment on Build (including ParallelSync)
        //playerName = ClonesManager.IsClone() ?
        //    $"Clone {Random.Range(100, 999)}" :
        //    PlayerPrefs.GetString("Profile_Name", "");

        playerManager.CmdReady(playerName);

        gameObject.SetActive(false);
    }
}
