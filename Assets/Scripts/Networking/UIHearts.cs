using UnityEngine;
using UnityEngine.UI;

public class UIHearts : MonoBehaviour
{
    public Image[] hearts; // Assign in Inspector (e.g., 4 heart icons)

    

    public void SetHearts(int amount)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < amount);
        }
    }
}
