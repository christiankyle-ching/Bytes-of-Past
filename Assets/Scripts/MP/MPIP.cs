using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class MPIP : NetworkBehaviour
{
    private TextMeshProUGUI _textObj;

    private void Start()
    {
        _textObj = transform.Find("IP").GetComponent<TextMeshProUGUI>();
        ShowIP();
    }

    private void ShowIP()
    {
        if (isServer)
        {
            try
            {
                _textObj.text = GetIPv4();
            }
            catch
            {
                _textObj.text = NetworkManager.singleton.networkAddress;
            }
        }
        else
        {
            _textObj.text = connectionToServer.address;
        }
    }

    private string GetIPv4()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}