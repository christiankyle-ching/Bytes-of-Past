using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenshotShare : MonoBehaviour
{
    Button _button;
    TextMeshProUGUI _buttonText;

    private void Start()
    {
        _button = GetComponent<Button>();
        _buttonText = _button.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        string playerName = StaticData.Instance.GetPlayerFullName();

        _button.interactable = playerName != string.Empty;
        _buttonText.text = playerName == string.Empty ? "Cannot Share Without Name" : "Share";

        _button.onClick.AddListener(Share);
    }

    public void Share()
    {
        StartCoroutine(TakeScreenshot());
    }

    IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D res = ScreenCapture.CaptureScreenshotAsTexture();

        new NativeShare().AddFile(res)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }

}
