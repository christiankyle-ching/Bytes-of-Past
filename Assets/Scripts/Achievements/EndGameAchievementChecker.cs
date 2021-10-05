using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameAchievementChecker : MonoBehaviour
{
    public GameObject canvas;
    public GameObject container;
    public GameObject achievementItemPrefab;

    private void Start()
    {
        canvas.SetActive(false);
    }

    public void ViewAcquiredAchievements(GameData gameData)
    {
        AchievementData[] acquiredAchievements = AchievementChecker.CheckAchievements(gameData);

        canvas.SetActive(true);

        Debug.Log($"Acquired Achievements: {acquiredAchievements.Length}");
        foreach (AchievementData ach in acquiredAchievements)
        {
            Debug.Log($"{ach.title}: {ach.description}");
            AchievementLoader.AddItem(ach, container.transform, achievementItemPrefab);
        }
    }
}
