using UnityEngine;

public class Settings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Hides the panel (GameObject this script is attached to)
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    // Shows the panel (GameObject this script is attached to)
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }
}
