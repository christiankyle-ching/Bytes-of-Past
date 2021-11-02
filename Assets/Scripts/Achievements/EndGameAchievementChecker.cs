using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameAchievementChecker : MonoBehaviour
{
    public GameObject achievementItemPrefab;

    public void ViewAcquiredAchievements(GameData gameData)
    {
        AchievementData[] acquiredAchievements = AchievementChecker.CheckAchievements(gameData);

        Debug.Log($"Acquired Achievements: {acquiredAchievements.Length}");
        foreach (AchievementData ach in acquiredAchievements)
        {
            Debug.Log($"{ach.title}: {ach.description}");
            AchievementLoader.AddItem(ach, transform, achievementItemPrefab);
        }
    }
}
