using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CanvasHUD : MonoBehaviour
{
    public Button btnHost;
    public Button btnJoin;
    public Button btnStop;

    public GameObject PanelStart;
    public GameObject PanelStop;

    public TMP_InputField inputFieldAddress;
    public TextMeshProUGUI ipAddress;
    public Text serverText;
    public Text clientText;

    private void Start()
    {
        ValueChangeCheck();
        inputFieldAddress.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

#if UNITY_EDITOR
        inputFieldAddress.text = "localhost";
#else
        inputFieldAddress.text = "192.168.";
#endif

        //Make sure to attach these Buttons in the Inspector
        btnHost.onClick.AddListener(HostGame);
        btnJoin.onClick.AddListener(JoinGame);
        btnStop.onClick.AddListener(CancelConnect);

        SetupCanvas();
    }

    private void HostGame()
    {
        SoundManager.Instance.PlayClickedSFX();

        NetworkManager.singleton.StartHost();
        SetupCanvas();
    }

    private void JoinGame()
    {
        SoundManager.Instance.PlayClickedSFX();

        NetworkManager.singleton.StartClient();
        SetupCanvas();
    }

    private void CancelConnect()
    {
        SoundManager.Instance.PlayClickedSFX();

        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.active)
        {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        SetupCanvas();
    }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        NetworkManager.singleton.networkAddress = inputFieldAddress.text;
        ipAddress.text = NetworkManager.singleton.networkAddress;

        btnJoin.interactable = inputFieldAddress.text != string.Empty;
    }

    public void SetupCanvas()
    {
        // Here we will dump majority of the canvas UI that may be changed.
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                // CLIENT ATTEMPTING TO CONNECT
                PanelStart.SetActive(false);
                PanelStop.SetActive(true);
                //clientText.text = "Connecting to " + NetworkManager.singleton.networkAddress + "..";
            }
            else
            {
                PanelStart.SetActive(true);
                PanelStop.SetActive(false);
            }
        }
        else
        {
            PanelStart.SetActive(false);
            PanelStop.SetActive(true);

            // server / client status message
            if (NetworkServer.active)
            {
                //serverText.text = "Server: active. Transport: " + Transport.activeTransport;
            }
            if (NetworkClient.isConnected)
            {
                //clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }
}
