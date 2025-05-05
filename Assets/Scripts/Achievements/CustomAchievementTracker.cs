using UnityEngine;

public class CustomAchievementTracker : MonoBehaviour
{
    // Movement tracking
    private bool movedLeft = false;
    private bool movedRight = false;
    private bool jumped = false;
    private bool movementAchievementUnlocked = false;

    // Combo kill tracking (optional for future)
    private int cleanKillStreak = 0;
    private bool comboAchievementUnlocked = false;

    void Start()
    {
        // Reset the movement achievement for testing
        PlayerPrefs.DeleteKey("first_steps");
        PlayerPrefs.Save();
        Debug.Log("Reset 'first_steps' for testing.");
    }

    void Update()
    {
        TrackMovement();
        CheckMovementAchievement();
    }

    private void TrackMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");

        if (move < 0) movedLeft = true;
        if (move > 0) movedRight = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumped = true;
        }
    }

    private void CheckMovementAchievement()
    {
        if (!movementAchievementUnlocked && movedLeft && movedRight && jumped)
        {
            movementAchievementUnlocked = true;
            Debug.Log("UNLOCKING: first_steps");
            AchievementManager.Instance.UnlockAchievement("first_steps");
        }
    }

    // Call this from enemy logic later
    public void OnEnemyKilled(bool correctClick)
    {
        if (comboAchievementUnlocked) return;

        if (correctClick)
        {
            cleanKillStreak++;
            if (cleanKillStreak >= 5)
            {
                comboAchievementUnlocked = true;
                AchievementManager.Instance.UnlockAchievement("combo_kill_5_clean");
            }
        }
        else
        {
            cleanKillStreak = 0;
        }
    }
}
