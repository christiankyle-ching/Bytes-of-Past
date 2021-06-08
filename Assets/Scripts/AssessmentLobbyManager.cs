using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssessmentLobbyManager : MonoBehaviour
{
    public GameObject sceneLoader;

    private SceneLoader _sceneLoader;

    // SELECTED
    public GameObject

            btnComputer,
            btnNetworking,
            btnSoftware;

    Vector2 normalScale = new Vector2(1f, 1f);

    Vector2 selectedScale = new Vector2(1.05f, 1.05f);

    int selectedTopic = 0;

    void Awake()
    {
        _sceneLoader = sceneLoader.GetComponent<SceneLoader>();
        OnTopicSelected(selectedTopic);
    }

    public void OnTopicSelected(int topic)
    {
        selectedTopic = topic;

        switch ((TOPIC) topic)
        {
            case TOPIC.Computer:
                SetButtonAppearance(btnComputer, true);
                SetButtonAppearance(btnNetworking, false);
                SetButtonAppearance(btnSoftware, false);
                break;
            case TOPIC.Networking:
                SetButtonAppearance(btnComputer, false);
                SetButtonAppearance(btnNetworking, true);
                SetButtonAppearance(btnSoftware, false);
                break;
            case TOPIC.Software:
                SetButtonAppearance(btnComputer, false);
                SetButtonAppearance(btnNetworking, false);
                SetButtonAppearance(btnSoftware, true);
                break;
        }
    }

    void SetButtonAppearance(GameObject button, bool isSelected)
    {
        button.transform.localScale = isSelected ? selectedScale : normalScale;
        button.GetComponent<Image>().color = isSelected ? Color.white : Color.white;
    }

    public void StartMatch()
    {
        _sceneLoader.LoadAssessment((TOPIC) selectedTopic, 1);
    }
}
