﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class StudentProfileHandler : MonoBehaviour
{
    public TMP_InputField txtStudentName;
    public TMP_Dropdown dropdownStudentSection;

    Color defaultColor = Color.white;

    public GameObject nameErrorTooltip;
    public GameObject nameEmptyTooltip;

    const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
    const string SYMBOLS = ", ";

    public void OnNameValueChanged(string text)
    {
        // Don't allow other characters
        if (text.Length > 0)
        {
            string lastChar = text.Substring(text.Length - 1, 1).ToLower();
            if (!(ALPHABET.Contains(lastChar) || SYMBOLS.Contains(lastChar)))
            {
                txtStudentName.text = text.Substring(0, text.Length - 1);
                return;
            }
        }

        if (StaticData.IsPlayerNameValid(text))
        {
            txtStudentName.textComponent.color = defaultColor;
        }
        else
        {
            txtStudentName.textComponent.color = Color.red;
        }
    }

    public void OnNameEndEdit(string text)
    {
        if (text == string.Empty)
        {
            nameEmptyTooltip.SetActive(true);
            nameErrorTooltip.SetActive(false);
        }
        else
        {
            bool isValid = StaticData.IsPlayerNameValid(text);

            if (isValid) StaticData.Instance.SetPlayerName(text);

            nameErrorTooltip.SetActive(!isValid);
            nameEmptyTooltip.SetActive(false);
        }
    }

    public void OnValueChangedSection(int sectionIndex)
    {
        StaticData.Instance.SetPlayerSection((StudentSection)sectionIndex);
    }

    void Start()
    {
        defaultColor = txtStudentName.textComponent.color;

        LoadStudentSections();

        // Load Saved
        txtStudentName.text = StaticData.Instance.GetDBPlayerName();
        dropdownStudentSection.value = StaticData.Instance.GetPlayerSection();

        // Check on start
        OnNameEndEdit(txtStudentName.text);
    }

    public void LoadStudentSections()
    {
        dropdownStudentSection.ClearOptions();
        dropdownStudentSection.AddOptions(StaticData.Instance.GetPlayerSections());
    }


}
