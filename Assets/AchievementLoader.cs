using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementLoader : MonoBehaviour
{
    public GameObject achievementsContainer;
    public GameObject achievementItemPrefab;

    void Start()
    {
        LoadAchievements();
    }

    void Update()
    {

    }

    void LoadAchievements()
    {
        // Single Player
        AchievementData[] spData = Resources.LoadAll<AchievementData>("Achievements/SP");
        foreach (AchievementData data in spData)
        {
            AddItem(data);
        }

        // Multiplayer
        AchievementData[] mpData = Resources.LoadAll<AchievementData>("Achievements/MP");
        foreach (AchievementData data in mpData)
        {
            AddItem(data);
        }
    }

    void AddItem(AchievementData data)
    {
        GameObject item = Instantiate(achievementItemPrefab, achievementsContainer.transform);
        item.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = data.title;
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = data.description;

        if (!data.isDone)
        {
            item.transform.Find("Image").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
