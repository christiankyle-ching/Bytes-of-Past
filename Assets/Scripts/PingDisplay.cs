using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class PingDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image wifiIcon;
    public TextMeshProUGUI msText;

    [Header("Parameters")]
    public float updateInterval = 1f;
    float _intervalLeft;

    private void Awake()
    {
        _intervalLeft = updateInterval;
    }

    private void Start()
    {
        UpdatePing();
    }

    private void Update()
    {
        _intervalLeft -= Time.deltaTime;

        if (_intervalLeft < 0)
        {
            UpdatePing();

            _intervalLeft = updateInterval;
        }
    }

    void UpdatePing()
    {
        double ping = Math.Round(NetworkTime.rtt * 1000);

        msText.text = $": {ping} ms";

        if (ping < 100)
        {
            wifiIcon.color = Color.green;
        }
        else
        {
            wifiIcon.color = Color.red;
        }
    }
}
