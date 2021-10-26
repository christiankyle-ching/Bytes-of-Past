using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentProfileHandler : MonoBehaviour
{
    public GameObject

            txtStudentName,
            dropdownStudentSection;

    public void OnEndEditName(string text)
    {
        PlayerPrefs.SetString("Profile_Name", text);
    }

    public void OnValueChangedSection(int sectionIndex)
    {
        PlayerPrefs.SetInt("Profile_Section", sectionIndex);
    }

    void Start()
    {
        txtStudentName.GetComponent<TMP_InputField>().text =
            PlayerPrefs.GetString("Profile_Name", "");

        dropdownStudentSection.GetComponent<TMP_Dropdown>().value =
            PlayerPrefs.GetInt("Profile_Section", 0);
    }
}
