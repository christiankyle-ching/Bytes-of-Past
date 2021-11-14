using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileLoader : MonoBehaviour
{
    [Header("UI References")]
    public AvatarLoader avatar;
    public TextMeshProUGUI playerName;

    private void Start()
    {
        avatar.SetPreview(StaticData.Instance.GetPlayerAvatar());
        playerName.text = StaticData.Instance.GetPlayerName();
    }
}
