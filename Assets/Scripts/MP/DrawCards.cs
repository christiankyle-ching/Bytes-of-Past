using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCards : NetworkBehaviour
{
    private PlayerManager playerManager;

    public void Draw()
    {
        NetworkIdentity ni = NetworkClient.connection.identity;
        playerManager = ni.GetComponent<PlayerManager>();
        playerManager.CmdDealCards();
    }
}
