﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PREFTYPES
{
    Int, Float, String
}

public class SetActiveBasedOnPlayerPref : MonoBehaviour
{
    public string playerPrefKey = "";
    public string playerPrefDefaultValue = "";
    public string playerPrefValueToActive = "";
    public string playerPrefValueToInactive = "";
    public PREFTYPES playerPrefType = PREFTYPES.Int;

    void Start()
    {
        try
        {
            switch (playerPrefType)
            {
                case PREFTYPES.Int:
                    int dInt = int.Parse(playerPrefDefaultValue);
                    int aInt = int.Parse(playerPrefValueToActive);
                    gameObject.SetActive(PlayerPrefs.GetInt(playerPrefKey, dInt) == aInt);
                    break;
                case PREFTYPES.Float:
                    float dFloat = float.Parse(playerPrefDefaultValue);
                    float aFloat = float.Parse(playerPrefValueToActive);
                    gameObject.SetActive(PlayerPrefs.GetFloat(playerPrefKey, dFloat) == aFloat);
                    break;
                case PREFTYPES.String:
                    string dString = playerPrefDefaultValue;
                    string aString = playerPrefValueToActive;
                    gameObject.SetActive(PlayerPrefs.GetString(playerPrefKey, dString) == aString);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void DontShowAgain()
    {
        if (playerPrefValueToInactive != string.Empty)
        {
            try
            {
                switch (playerPrefType)
                {
                    case PREFTYPES.Int:
                        int iInt = int.Parse(playerPrefValueToInactive);
                        PlayerPrefs.SetInt(playerPrefKey, iInt);
                        break;
                    case PREFTYPES.Float:
                        float iFloat = float.Parse(playerPrefValueToInactive);
                        PlayerPrefs.SetFloat(playerPrefKey, iFloat);
                        break;
                    case PREFTYPES.String:
                        string iString = playerPrefValueToInactive;
                        PlayerPrefs.SetString(playerPrefKey, iString);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        DestroyGO();
    }

    public void DestroyGO()
    {
        gameObject.SetActive(false);
    }
}
