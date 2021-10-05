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


    void LoadAchievements()
    {
        AchievementData[] achievements = AchievementChecker.GetAchievements();

        foreach (AchievementData data in achievements)
        {
            AddItem(data, achievementsContainer.transform, achievementItemPrefab);
        }
    }

    public static void AddItem(AchievementData data, Transform parent, GameObject prefab)
    {
        GameObject item = Instantiate(prefab, parent);

        Transform content = item.transform.GetChild(0);
        content.Find("TextCol").Find("Title").GetComponent<TextMeshProUGUI>().text = data.title;
        content.Find("TextCol").Find("Description").GetComponent<TextMeshProUGUI>().text = data.description;

        if (!data.isDone)
        {
            content.Find("ImageCol").Find("Image").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
