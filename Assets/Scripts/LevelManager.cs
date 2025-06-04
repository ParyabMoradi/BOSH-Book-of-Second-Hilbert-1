using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject levelDoor;

    private int defeatedEnemies = 0;
    public int enemiesToDefeat = 0; // Threshold to open the door

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }



    public void IncreaseDefeatedEnemies()
    {
        defeatedEnemies += 1;
        if (defeatedEnemies >= enemiesToDefeat) // Assuming 10 is the threshold to open the door
        {
            OpenLevelDoor();
        }
    }

    public void OpenLevelDoor()
    {
        if (levelDoor != null)
        {
            levelDoor.SetActive(true);
            Debug.Log("Level door opened!");
        }
        else
        {
            Debug.LogWarning("Level door not set!");
        }
    }

}
