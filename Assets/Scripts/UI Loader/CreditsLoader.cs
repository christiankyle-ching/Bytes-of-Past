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
    public TextMeshProUGUI txtSFX;
    public TextMeshProUGUI txtUI;
    public TextMeshProUGUI txtImagesComputer;
    public TextMeshProUGUI txtImagesSoftware;
    public TextMeshProUGUI txtImagesNetworking;

    [Header("Auto-Scroll")]
    public float scrollSensitivity = 1000f;

    private TextAsset chm;
    private TextAsset music;
    private TextAsset sfx;
    private TextAsset ui;
    private TextAsset imagesComputer;
    private TextAsset imagesSoftware;
    private TextAsset imagesNetworking;

    bool isScrolling = false;

    private void Awake()
    {
        chm = Resources.Load<TextAsset>("Licenses/chm");
        music = Resources.Load<TextAsset>("Licenses/bgm");
        sfx = Resources.Load<TextAsset>("Licenses/sfx");
        ui = Resources.Load<TextAsset>("Licenses/ui");
        imagesComputer = Resources.Load<TextAsset>("Licenses/computer_images");
        imagesSoftware = Resources.Load<TextAsset>("Licenses/networking_images");
        imagesNetworking = Resources.Load<TextAsset>("Licenses/software_images");

        txtCHM.text = chm.text;
        txtMusic.text = music.text;
        txtSFX.text = sfx.text;
        txtUI.text = ui.text;
        txtImagesComputer.text = imagesComputer.text;
        txtImagesSoftware.text = imagesSoftware.text;
        txtImagesNetworking.text = imagesNetworking.text;
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());

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
            }
            else
            {
                isScrolling = false;
            }
        }
    }
}
