using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DrawCards : NetworkBehaviour
{
    private PlayerManager playerManager;

    public void Draw()
    {
        // This draws cards
        NetworkIdentity ni = NetworkClient.connection.identity;
        playerManager = ni.GetComponent<PlayerManager>();
        playerManager.CmdReady();

        gameObject.SetActive(false);
    }
}
