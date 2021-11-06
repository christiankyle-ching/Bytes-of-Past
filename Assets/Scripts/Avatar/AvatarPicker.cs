using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPicker : MonoBehaviour
{
    [Header("UI References")]
    public AvatarLoader avatarLoader;
    public Button btnNext, btnPrev;

    int selectedAvatarIndex;
    int avatarLength;

    private void Start()
    {
        selectedAvatarIndex = (int)StaticData.Instance.GetPlayerAvatar();
        avatarLength = Enum.GetValues(typeof(Avatar)).Length;

        avatarLoader.SetPreview((Avatar)selectedAvatarIndex);

        btnNext.onClick.AddListener(NextAvatar);
        btnPrev.onClick.AddListener(PrevAvatar);
    }

    private void NextAvatar()
    {
        SoundManager.Instance.PlayClickedSFX();

        if (selectedAvatarIndex + 1 < avatarLength)
        {
            selectedAvatarIndex += 1;
        }
        else
        {
            selectedAvatarIndex = 0; // Back to start
        }

        Avatar av = (Avatar)selectedAvatarIndex;

        avatarLoader.SetPreview(av);
        StaticData.Instance.SetPlayerAvatar(av);
    }

    private void PrevAvatar()
    {
        SoundManager.Instance.PlayClickedSFX();

        if (selectedAvatarIndex - 1 >= 0)
        {
            selectedAvatarIndex -= 1;
        }
        else
        {
            selectedAvatarIndex = avatarLength - 1; // Back to end
        }

        Avatar av = (Avatar)selectedAvatarIndex;

        avatarLoader.SetPreview(av);
        StaticData.Instance.SetPlayerAvatar(av);
    }
}
