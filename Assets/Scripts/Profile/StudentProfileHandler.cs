using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentProfileHandler : MonoBehaviour
{
    // TODO: Set in Prod
    int minNameLength = 4;

    public TMP_InputField txtStudentName;
    public TMP_Dropdown dropdownStudentSection;

    Color defaultColor = Color.white;

    public GameObject nameErrorTooltip;
    public GameObject nameEmptyTooltip;

    public void OnNameValueChanged(string text)
    {
        if (IsNameValid(text))
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
            bool isValid = IsNameValid(text);

            if (isValid) PlayerPrefs.SetString("Profile_Name", text);

            nameErrorTooltip.SetActive(!isValid);
            nameEmptyTooltip.SetActive(false);
        }
    }

    public void OnValueChangedSection(int sectionIndex)
    {
        StaticData.Instance.SetPlayerSection(sectionIndex);
    }

    void Start()
    {
        defaultColor = txtStudentName.textComponent.color;

        // Load Saved
        txtStudentName.text = StaticData.Instance.GetPlayerName();
        dropdownStudentSection.value = StaticData.Instance.GetPlayerSection();

        // Check on start
        OnNameEndEdit(txtStudentName.text);
    }

    public bool IsNameValid(string name)
    {
        return name.Length >= minNameLength;
    }
}
