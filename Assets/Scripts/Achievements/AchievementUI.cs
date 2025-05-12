using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AchievementUI : MonoBehaviour
{
    public static AchievementUI Instance;

    [Header("UI Elements")]
    public GameObject panel;            // The parent panel for the popup
    public Image iconImage;            // UI Image to show the achievement icon
    public TMP_Text descriptionText;   // Text for the description

    [Header("Popup Settings")]
    public float displayTime = 3f;     // Duration the popup stays visible

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    /// <summary>
    /// Show the achievement popup with provided data.
    /// </summary>
    public Animator animator; // Assign in Inspector

    public void ShowAchievement(AchievementData data)
    {
        Debug.Log("Showing popup for: " + data.id); // <== ADD THIS

        iconImage.sprite = data.icon;
        descriptionText.text = data.description;

        StopAllCoroutines();
        StartCoroutine(DisplayPopup());

        if (animator != null)
        {
            animator.Play("AchievementPopup_Show", 0, 0f);
        }
}



    /// <summary>
    /// Coroutine to show then hide the panel.
    /// </summary>
    private IEnumerator DisplayPopup()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        panel.SetActive(false);
    }
}
