using UnityEngine;
using UnityEngine.UI; // Or use TMPro if you're using TextMeshPro

public class TextTrigger : MonoBehaviour
{
    public GameObject textObject; // Assign the UI Text GameObject in Inspector

    void Start()
    {
        if (textObject != null)
            textObject.SetActive(false); // Hide at start
    }

    void OnTriggerEnter(Collider2D other) // Use OnTriggerEnter2D if using 2D
    {
        if (other.CompareTag("Player"))
        {
            if (textObject != null)
                textObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider2D other) // Use OnTriggerExit2D for 2D
    {
        if (other.CompareTag("Player"))
        {
            if (textObject != null)
                textObject.SetActive(false);
        }
    }
}
