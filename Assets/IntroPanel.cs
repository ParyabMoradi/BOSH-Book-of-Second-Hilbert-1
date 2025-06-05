using UnityEngine;

public class IntroPanel : MonoBehaviour
{
    // Show the panel
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    // Hide the panel
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
