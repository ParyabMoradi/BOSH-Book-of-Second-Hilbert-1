using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<AchievementData> allAchievements; // Add them in Inspector
    private HashSet<string> unlockedAchievements = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockAchievement(string id)
    {
        if (unlockedAchievements.Contains(id)) return;

        AchievementData data = allAchievements.Find(a => a.id == id);
        if (data != null)
        {
            unlockedAchievements.Add(id);
            PlayerPrefs.SetInt(id, 1);
            PlayerPrefs.Save();

            AchievementUI.Instance.ShowAchievement(data);
        }
    }

    private void LoadAchievements()
    {
        foreach (var achievement in allAchievements)
        {
            if (PlayerPrefs.GetInt(achievement.id, 0) == 1)
            {
                unlockedAchievements.Add(achievement.id);
            }
        }
    }

    public bool IsUnlocked(string id) => unlockedAchievements.Contains(id);
}
