using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadSystem
{
    static string statisticsDataPath = "/profile_stats.dat";

    public static void SaveProfileStatisticsData(
        ProfileStatisticsData statsData
    )
    {
        try
        {
            string path = Application.persistentDataPath + statisticsDataPath;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, statsData);

            stream.Close();
        }
        catch (IOException)
        {
            Debug.Log("Error saving player data.");
        }

    }

    public static ProfileStatisticsData LoadProfileStatisticsData()
    {
        try
        {
            string path = Application.persistentDataPath + statisticsDataPath;

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                ProfileStatisticsData statsData =
                    formatter.Deserialize(stream) as ProfileStatisticsData;

                stream.Close();

                return statsData;
            }
            else
            {
                Debug.LogError("No Profile Data found in: " + path);

                Debug.Log("Profile Data: Creating a default one.");

                // Create an empty one
                ProfileStatisticsData newProfileStatisticsData =
                    new ProfileStatisticsData();
                SaveLoadSystem.SaveProfileStatisticsData(newProfileStatisticsData);
                return newProfileStatisticsData;
            }
        }
        catch (IOException)
        {
            Debug.Log("Error saving player data.");

            // Create an empty one
            ProfileStatisticsData newProfileStatisticsData =
                new ProfileStatisticsData();
            SaveLoadSystem.SaveProfileStatisticsData(newProfileStatisticsData);
            return newProfileStatisticsData;
        }
    }

    public static void ResetProfileData(bool resetPrefs = false)
    {
        // Clear all Player Prefs
        if (resetPrefs) PlayerPrefs.DeleteAll();

        // Create an empty one
        ProfileStatisticsData newProfileStatisticsData =
            new ProfileStatisticsData();
        SaveLoadSystem.SaveProfileStatisticsData(newProfileStatisticsData);
    }
}
