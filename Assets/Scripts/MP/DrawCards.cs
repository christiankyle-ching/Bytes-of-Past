using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DrawCards : NetworkBehaviour
{
    private PlayerManager playerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 2. BUT only works for host!
        //Draw();
    }

    public void Draw()
    {
        // 1. This works fine when clicked via Button
        NetworkIdentity ni = NetworkClient.connection.identity;
        playerManager = ni.GetComponent<PlayerManager>();
        playerManager.CmdReady();

        gameObject.SetActive(false);
    }
}
