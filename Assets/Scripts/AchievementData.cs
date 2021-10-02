using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "ScriptableObjects/AchievementData")]
public class AchievementData : ScriptableObject
{
    public string title;
    public string description;
    public bool isDone;
}
