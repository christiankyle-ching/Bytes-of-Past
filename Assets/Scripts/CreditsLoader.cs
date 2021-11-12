using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsLoader : MonoBehaviour
{
    [Header("Child References")]
    public ScrollRect container;
    public GameObject panel;
    public TextMeshProUGUI txtCHM;
    public TextMeshProUGUI txtMusic;
    public TextMeshProUGUI txtImagesComputer;
    public TextMeshProUGUI txtImagesSoftware;
    public TextMeshProUGUI txtImagesNetworking;

    [Header("Auto-Scroll")]
    public float scrollSensitivity = 1000f;

    private TextAsset chm;
    private TextAsset music;
    private TextAsset imagesComputer;
    private TextAsset imagesSoftware;
    private TextAsset imagesNetworking;

    bool isScrolling = false;

    private void Awake()
    {
        chm = Resources.Load<TextAsset>("Licenses/license_chm");
        music = Resources.Load<TextAsset>("Licenses/license_music");
        imagesComputer = Resources.Load<TextAsset>("Licenses/license_images_computer");
        imagesSoftware = Resources.Load<TextAsset>("Licenses/license_images_networking");
        imagesNetworking = Resources.Load<TextAsset>("Licenses/license_images_software");

        txtCHM.text = chm.text;
        txtMusic.text = music.text;
        txtImagesComputer.text = imagesComputer.text;
        txtImagesSoftware.text = imagesSoftware.text;
        txtImagesNetworking.text = imagesNetworking.text;
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());

        txtCHM.ForceMeshUpdate();
        txtMusic.ForceMeshUpdate();
        txtImagesComputer.ForceMeshUpdate();
        txtImagesSoftware.ForceMeshUpdate();
        txtImagesNetworking.ForceMeshUpdate();

        Invoke(nameof(StartScrolling), 1f);
    }

    void StartScrolling()
    {
        isScrolling = true;
    }

    private void Update()
    {
        if (isScrolling)
        {
            if (container.verticalNormalizedPosition >= 0.01f)
            {
                container.velocity = new Vector2(0f, scrollSensitivity);
            } else
            {
                isScrolling = false;
            }
        }
    }
}
