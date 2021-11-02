using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementChecker
{
    public static AchievementData[] CheckAchievements(GameData gameData)
    {
        List<AchievementData> acquiredList = new List<AchievementData>();
        AchievementData[] achievements = GetAchievements();

        foreach (AchievementData achievement in achievements)
        {
            if (achievement.isDone) continue;

            if (VerifyAchievementAcquired(achievement.id, gameData))
            {
                achievement.isDone = true;
                acquiredList.Add(achievement);
            }
        }

        return acquiredList.ToArray(); // List of all acquired achievements in a single game session
    }

    private static bool VerifyAchievementAcquired(int achievementID, GameData gameData)
    {
        if (IsAchievementDone(achievementID)) return false;

        bool acquired = false;

        // Checker Functions : sets the value of acquired
        // if the achievement is just achieved in this game session (gameData)
        switch (achievementID)
        {
            case 0:
                acquired = gameData.gameMode == GAMEMODE.SinglePlayer &&
                    gameData.remainingLife == gameData.initialLife;
                break;
            case 1:
                acquired = gameData.accuracy == 0.5f;
                break;
            case 2:
                acquired = gameData.gameWon;
                break;
            case 3:
                acquired = gameData.accuracy >= 0.8f;
                break;
            case 4:
                acquired = gameData.gameMode == GAMEMODE.Multiplayer && gameData.gameWon;
                break;
            case 5:
                bool t1 = PrefsConverter.IntToBoolean(PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPlayed(TOPIC.Computer), 0));
                bool t2 = PrefsConverter.IntToBoolean(PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPlayed(TOPIC.Networking), 0));
                bool t3 = PrefsConverter.IntToBoolean(PlayerPrefs.GetInt(TopicUtils.GetPrefKey_IsPlayed(TOPIC.Software), 0));

                acquired = t1 && t2 && t3;
                break;
            case 6:
                acquired = gameData.gameMode == GAMEMODE.SinglePlayer && gameData.difficulty == DIFFICULTY.Easy && gameData.gameWon;
                break;
            case 7:
                acquired = gameData.gameMode == GAMEMODE.SinglePlayer && gameData.difficulty == DIFFICULTY.Medium && gameData.gameWon;
                break;
            case 8:
                acquired = gameData.gameMode == GAMEMODE.SinglePlayer && gameData.difficulty == DIFFICULTY.Hard && gameData.gameWon;
                break;
            case 9:
                string prefKey = "ACH09_CurrentWins";
                bool isRightGameParams =
                    gameData.gameMode == GAMEMODE.SinglePlayer &&
                    gameData.difficulty == DIFFICULTY.Hard &&
                    gameData.gameWon &&
                    gameData.remainingLife == gameData.initialLife;
                if (isRightGameParams)
                {
                    int currentWins = PlayerPrefs.GetInt(prefKey, 0);
                    PlayerPrefs.SetInt(prefKey, currentWins + 1);

                    acquired = currentWins >= 3;
                }
                break;
        }

        if (acquired) SetAchievementDone(achievementID);

        return acquired;
    }

    #region Getter / Setter
    public static AchievementData[] GetAchievements()
    {
        AchievementData[] achievements = Resources.LoadAll<AchievementData>("Achievements");

        // Check `isDone` status
        foreach (AchievementData item in achievements)
        {
            item.isDone = IsAchievementDone(item.id);
        }

        return achievements;
    }

    private static bool IsAchievementDone(int id)
    {
        return PlayerPrefs.GetInt(GetAchievementKey(id), 0) == 1;
    }

    private static void SetAchievementDone(int id)
    {
        PlayerPrefs.SetInt(GetAchievementKey(id), 1);
    }

    private static string GetAchievementKey(int id)
    {
        return $"Achievement_{id}";
    }
    #endregion
}
