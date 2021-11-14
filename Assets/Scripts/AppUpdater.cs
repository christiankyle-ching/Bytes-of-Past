using LogicUI.FancyTextRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AppUpdater : MonoBehaviour
{
    [Header("UI References")]
    public GameObject modal;
    public MarkdownRenderer txtUpdate;
    public Button btnUpdate;
    public Button btnClose;

    Animator anim;

    readonly string githubReleasesUrlApi =
        "https://api.github.com/repos/christiankyle-ching/Prototype--Bytes-of-Past/releases?per_page=1&page=1";

    readonly string githubReleasesUrl =
        "https://github.com/christiankyle-ching/Prototype--Bytes-of-Past/releases";

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        SetupCanvas(false);

        Invoke(nameof(CheckUpdates), 3f);

        btnUpdate.onClick.AddListener(GoToDownloads);
        btnClose.onClick.AddListener(() => SetupCanvas(false));
    }

    void SetupCanvas(bool enabled)
    {
        modal.SetActive(enabled);
        if (enabled) anim.SetTrigger("Fade");
    }

    void CheckUpdates()
    {
        StartCoroutine(GetLatestVersion((version, desc) =>
        {
            int curVersion = ParseVersion(GetCurrentVersion());
            int latestVersion = ParseVersion(version);

            // DEBUG: Show latest version
            //txtUpdate.Source = GetChangelog(version, desc);
            //SetupCanvas(true);

            if (latestVersion > curVersion)
            {
                txtUpdate.Source = GetChangelog(version, desc);
                SetupCanvas(true);
            }
        }));
    }

    public void GoToDownloads()
    {
        Application.OpenURL(githubReleasesUrl);
    }

    public string GetCurrentVersion()
    {
        return $"v{Application.version}";
    }

    // Callback(string version, string description)
    public IEnumerator GetLatestVersion(Action<string, string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(githubReleasesUrlApi))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            while (!webRequest.isDone) { yield return null; }

            if (webRequest.isHttpError)
            {
                callback("", "");
            }
            else
            {
                Regex rxVersion = new Regex(@"""tag_name"":\s*""(?<version>[a-z0-9.]*)""");
                Match matchVersion = rxVersion.Match(webRequest.downloadHandler.text);

                Regex rxDesc = new Regex(@"""body"":\s*""(?<desc>.*)""\s*}\s*]");
                Match matchDesc = rxDesc.Match(webRequest.downloadHandler.text);

                if (matchVersion.Success)
                {
                    string version = matchVersion.Groups["version"].Value;
                    string desc = matchDesc.Success ? matchDesc.Groups["desc"].Value : "";

                    callback(version, ParseDescription(desc));
                }
            }

            yield return null;
        }
    }

    private int ParseVersion(string text)
    {
        int version = -1;

        // String from Array
        string numOnly = new string(text.Where(c => char.IsDigit(c)).ToArray());

        int.TryParse(numOnly, out version);

        return version;
    }

    private string ParseDescription(string description)
    {
        return description
            .Replace("\\r\\n", "\n"); // Fix newlines
    }

    private string GetChangelog(string version, string desc)
    {
        return $"## {version} \n" +
                $"{desc}";
    }
}
