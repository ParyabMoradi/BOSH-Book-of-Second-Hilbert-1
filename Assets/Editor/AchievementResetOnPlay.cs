using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class AchievementResetOnPlay
{
    static AchievementResetOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // When entering Play Mode
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Reset specific achievement
            PlayerPrefs.DeleteKey("first_steps"); // reset achievement "first steps"
            PlayerPrefs.Save();

            Debug.Log(" 'first steps' achievement reset on Play Mode start.");
        }
    }
}
