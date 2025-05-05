using UnityEngine;

public class SimpleMovementTracker : MonoBehaviour
{
    private bool movedLeft = false;
    private bool movedRight = false;
    private bool jumped = false;
    private bool achievementUnlocked = false;

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        if (move < 0) movedLeft = true;
        if (move > 0) movedRight = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumped = true;
        }

        if (!achievementUnlocked && movedLeft && movedRight && jumped)
        {
            achievementUnlocked = true;
            AchievementManager.Instance.UnlockAchievement("first_steps");
        }

        // DEBUG: to test the popup again, press R in player mode to clear cache
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll(); // 🔥 Clears all saved achievement data
            PlayerPrefs.Save();
            Debug.Log("All achievements reset");
        }
    }
}
