using LogicUI.FancyTextRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AppUpdater : MonoBehaviour
{
    [Header("UI References")]
    public GameObject updateModal;
    public GameObject checkingModal;
    public MarkdownRenderer txtUpdate;
    public Button btnUpdate;
    public Button btnClose;

    string _downloadUrl = "";

    readonly string githubReleasesUrlApi =
        "https://api.github.com/repos/christiankyle-ching/Prototype--Bytes-of-Past/releases?per_page=1&page=1";

    //readonly string githubReleasesUrl =
    //    "https://github.com/christiankyle-ching/Prototype--Bytes-of-Past/releases";

    // Start is called before the first frame update
    void Start()
    {
        updateModal.SetActive(false);
        checkingModal.SetActive(false);

        StartCoroutine(CheckUpdatesAtStart());

        btnUpdate.onClick.AddListener(GoToDownloads);
        btnClose.onClick.AddListener(() => ShowUpdateModal(false));
    }

    public IEnumerator CheckUpdatesAtStart()
    {
        yield return new WaitForSecondsRealtime(2f);

        CheckUpdates(false);
    }

    public void CheckUpdates(bool forced = false)
    {
        if (forced)
        {
            StartCoroutine(FetchAndCompareLatestVersion());
            StaticData.Instance.checkedUpdates = true;
        }
        else
        {
            if (!StaticData.Instance.checkedUpdates)
            {
                StartCoroutine(FetchAndCompareLatestVersion());
                StaticData.Instance.checkedUpdates = true;
            }
        }
    }

    public void GoToDownloads()
    {
        Application.OpenURL(_downloadUrl);
    }

    public string GetCurrentVersion()
    {
        return $"v{Application.version}";
    }

    // Callback(string version, string description)
    public IEnumerator FetchAndCompareLatestVersion()
    {
        Debug.Log("UPDATE: Checking...");
        ShowCheckingModal(true);

        UnityWebRequest webRequest = UnityWebRequest.Get(githubReleasesUrlApi);

        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        //while (!webRequest.isDone) { yield return null; }

        if (webRequest.isNetworkError)
        {
            //Debug.Log($"UPDATE: System Error - {webRequest.error}");
            ShowCheckingModal(true, "Internet Unavailable");
        }
        else if (webRequest.isHttpError)
        {
            //Debug.Log($"UPDATE: HTTP Error - {webRequest.error}");
            ShowCheckingModal(true, "Something went wrong");
        }
        else
        {
            ShowUpdate(webRequest.downloadHandler.text);
        }

        yield return null;

    }

    async void ShowUpdate(string jsonData)
    {
        if (jsonData == string.Empty)
        {
            ShowCheckingModal(false);
            return;
        }

        Regex rxVersion = new Regex(@"""tag_name"":\s*""(?<version>[a-z0-9.]*)""");
        Regex rxDesc = new Regex(@"""body"":\s*""(?<desc>.*)""\s*}\s*]");
        Regex rxDownload = new Regex(@"""browser_download_url"":\s*""(?<url>[^""]*)""");

        Match matchVersion = await Task.Run(() => rxVersion.Match(jsonData));
        Match matchDesc = await Task.Run(() => rxDesc.Match(jsonData));
        Match matchDownload = await Task.Run(() => rxDownload.Match(jsonData));

        if (matchVersion.Success)
        {
            string latestVersionString = matchVersion.Groups["version"].Value;
            string desc = matchDesc.Success ? matchDesc.Groups["desc"].Value : "";
            string url = matchDownload.Success ? matchDownload.Groups["url"].Value : "";

            int curVersion = ParseVersion(GetCurrentVersion());
            int latestVersion = ParseVersion(latestVersionString);

            _downloadUrl = url;

            // DEBUG: Show latest version
            //txtUpdate.Source = GetChangelog(version, desc);
            //SetupCanvas(true);

            if (latestVersion > curVersion)
            {
                txtUpdate.Source = GetChangelog(latestVersionString, desc);
                ShowUpdateModal(true);
                ShowCheckingModal(false);

                Debug.Log($"UPDATE: Available ({latestVersion}>{curVersion})");
            }
            else
            {
                Debug.Log($"UPDATE: Already latest version ({latestVersion}=={curVersion})");
                ShowCheckingModal(false, "No Updates", true);
            }
        }
        else
        {
            Debug.Log("UPDATE: Regex Error");
            ShowCheckingModal(false);
        }
    }

    #region ------------------------------ UTILS ------------------------------

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

    private void ShowUpdateModal(bool enabled)
    {
        updateModal.SetActive(enabled);
    }

    private void ShowCheckingModal(bool enabled, string errorMessage = "", bool success = false)
    {
        if (errorMessage != string.Empty)
        {
            // If there's an error, always disable afterwards
            checkingModal.GetComponentInChildren<TextEllipsisAnimation>().enabled = false;
            checkingModal.GetComponentInChildren<TextMeshProUGUI>().color = success ? Color.green : Color.yellow;
            checkingModal.GetComponentInChildren<TextMeshProUGUI>().text = errorMessage;

            DisableModal(checkingModal);
        }
        else
        {
            if (enabled)
            {
                checkingModal.SetActive(true);
            }
            else
            {
                DisableModal(checkingModal);
            }
        }
    }

    private void DisableModal(GameObject modal)
    {
        modal.GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
        modal.GetComponentInChildren<CanvasGroup>().interactable = false;

        modal.GetComponentInChildren<Animator>().SetTrigger("FadeOut");
    }

    #endregion
}
