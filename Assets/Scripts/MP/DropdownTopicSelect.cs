using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownTopicSelect : MonoBehaviour
{
    private HistoryTopic[] topics =
    {
        HistoryTopic.COMPUTER,
        HistoryTopic.NETWORKING,
        HistoryTopic.SOFTWARE
    };

    private TMP_Dropdown dropdown;
    private StaticData staticData;

    private void Start()
    {
        staticData = StaticData.Instance;

        // Load first before attaching listeners
        dropdown = GetComponent<TMP_Dropdown>();
        LoadTopics();
        LoadCurrentTopic();

        dropdown.onValueChanged.AddListener(UpdateValue);
    }

    private void UpdateValue(int index)
    {
        staticData.SetTopic(topics[index]);
    }

    private void LoadTopics()
    {
        dropdown.ClearOptions();

        List<string> topicNames = new List<string>();
        foreach (HistoryTopic topic in topics)
        {
            topicNames.Add(TopicUtils.GetName(topic));
        }

        dropdown.AddOptions(topicNames);
    }

    private void LoadCurrentTopic()
    {
        switch (staticData.SelectedTopic)
        {
            case HistoryTopic.COMPUTER:
                dropdown.value = 0;
                break;
            case HistoryTopic.NETWORKING:
                dropdown.value = 1;
                break;
            case HistoryTopic.SOFTWARE:
                dropdown.value = 2;
                break;
            default:
                dropdown.value = 0;
                break;
        }
    }
}
