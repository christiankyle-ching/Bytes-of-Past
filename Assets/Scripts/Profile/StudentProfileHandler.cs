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
        if (IsNameValid(text))
        {
            PlayerPrefs.SetString("Profile_Name", text);
        }
    }

    public void OnValueChangedSection(int sectionIndex)
    {
        PlayerPrefs.SetInt("Profile_Section", sectionIndex);
    }

    void Start()
    {
        defaultColor = txtStudentName.textComponent.color;

        // Load Saved
        txtStudentName.text = StaticData.Instance.GetPlayerName();
        dropdownStudentSection.value = StaticData.Instance.GetPlayerSection();
    }

    public bool IsNameValid(string name)
    {
        return name.Length >= minNameLength;
    }
}
