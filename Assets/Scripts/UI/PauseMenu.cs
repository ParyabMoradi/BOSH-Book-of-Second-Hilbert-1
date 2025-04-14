using UnityEngine;
using UnityEngine.SceneManagement; // Importing the SceneManagement namespace to manage scenes
using UnityEngine.UI; // Importing the UI namespace to manage UI elements

public class PauseMenu : MonoBehaviour
{
    // Static variable to track if the game is paused
    public static bool isPaused = false;

    // Reference to the pause menu UI GameObject
    public GameObject pauseMenuUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialization logic can go here if needed
        pauseMenuUI.SetActive(false); // Ensure the pause menu is not visible at the start
        Debug.Log("PauseMenu initialized. isPaused: " + isPaused);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check if the Escape key is pressed
        {
            if (isPaused) // If the game is currently paused
            {
                Debug.Log("Escape key pressed. Game is paused. Resuming...");
                Resume(); // Call the Resume method to unpause the game
            }
            else // If the game is not paused
            {
                Debug.Log("Escape key pressed. Game is not paused. Pausing...");
                Pause(); // Call the Pause method to pause the game
            }
        }
    }

    public void Resume() // Method to resume the game
    {
        if (!isPaused)
        {
            Debug.LogWarning("Resume called, but the game is not paused.");
            return;
        }

        pauseMenuUI.SetActive(false); // Deactivate the pause menu UI
        Time.timeScale = 1f; // Set the time scale to 1 (normal speed)
        isPaused = false; // Update the isPaused variable to false
        Debug.Log("Game resumed. isPaused: " + isPaused);
    }

    void Pause() // Method to pause the game
    {
        if (isPaused)
        {
            Debug.LogWarning("Pause called, but the game is already paused.");
            return;
        }

        pauseMenuUI.SetActive(true); // Activate the pause menu UI
        Time.timeScale = 0f; // Set the time scale to 0 (pause the game)
        isPaused = true; // Update the isPaused variable to true
        Debug.Log("Game paused. isPaused: " + isPaused);
    }

    public void GoToScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}