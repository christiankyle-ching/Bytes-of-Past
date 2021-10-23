using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownTopicSelect : MonoBehaviour
{
    private TOPIC[] topics =
    {
        TOPIC.Computer,
        TOPIC.Networking,
        TOPIC.Software
    };

    private TMP_Dropdown dropdown;
    private StaticData staticData;

    private void Start()
    {
        try
        {
            staticData =
                GameObject
                    .FindWithTag("Static Data")
                    .GetComponent<StaticData>();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("No Static Data detected: Run from Main Menu");
        }

        // Load first before attaching listeners
        dropdown = GetComponent<TMP_Dropdown>();
        LoadTopics();
        LoadCurrentTopic();

        dropdown.onValueChanged.AddListener(UpdateValue);
    }

    private void UpdateValue(int index)
    {
        staticData.SelectedTopic = topics[index];
    }

    private void LoadTopics()
    {
        dropdown.ClearOptions();

        List<string> topicNames = new List<string>();
        foreach (TOPIC topic in topics)
        {
            topicNames.Add(TopicUtils.GetName(topic));
        }

        dropdown.AddOptions(topicNames);
    }

    private void LoadCurrentTopic()
    {
        switch (staticData.SelectedTopic)
        {
            case TOPIC.Computer:
                dropdown.value = 0;
                break;
            case TOPIC.Networking:
                dropdown.value = 1;
                break;
            case TOPIC.Software:
                dropdown.value = 2;
                break;
            default:
                dropdown.value = 0;
                break;
        }
    }
}
