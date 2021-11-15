using LogicUI.FancyTextRendering;
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
    public MarkdownRenderer txtCHM;
    public MarkdownRenderer txtMusic;
    public MarkdownRenderer txtSFX;
    public MarkdownRenderer txtUI;
    public MarkdownRenderer txtImagesComputer;
    public MarkdownRenderer txtImagesSoftware;
    public MarkdownRenderer txtImagesNetworking;

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

        txtCHM.Source = chm.text;
        txtMusic.Source = music.text;
        txtSFX.Source = sfx.text;
        txtUI.Source = ui.text;
        txtImagesComputer.Source = imagesComputer.text;
        txtImagesSoftware.Source = imagesSoftware.text;
        txtImagesNetworking.Source = imagesNetworking.text;
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
