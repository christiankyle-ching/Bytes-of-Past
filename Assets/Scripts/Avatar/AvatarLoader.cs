using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Make sure filenames match these enums
// Also, make sure they are saved in the exact path below
// Assets/Resources/Avatars/
public enum Avatar
{
    MALE_0,
    MALE_1,
    FEMALE_0,
}

[RequireComponent(typeof(Image))]
public class AvatarLoader : MonoBehaviour
{
    static readonly string folderPath = "Avatars/";

    Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = Color.clear;
    }

    public void SetPreview(Avatar _av)
    {
        _image.color = Color.white;
        _image.sprite = GetAvatarSprite(_av);
    }

    public static Sprite GetAvatarSprite(Avatar _av)
    {
        return Resources.Load<Sprite>($"{folderPath}{_av}");
    }

    public static List<Sprite> GetAllAvatars()
    {
        List<Sprite> _avatars = new List<Sprite>();

        foreach (Avatar av in Enum.GetValues(typeof(Avatar)))
        {
            _avatars.Add(GetAvatarSprite(av)); // Need this to maintain order of Enums to Dropdown index
        }

        return _avatars;
    }
}
