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
        NetworkIdentity ni = NetworkClient.connection.identity;
        playerManager = ni.GetComponent<PlayerManager>();
        playerManager.CmdReady();

        GetComponent<Button>().interactable = false; // TODO: Might be able to do via Rpc to prevent something breaking
    }
}
