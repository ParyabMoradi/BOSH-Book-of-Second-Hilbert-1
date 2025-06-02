using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneHandler : MonoBehaviour
{
    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }

    // unload the scene
    public void UnloadScene(string level)
    {
        SceneManager.UnloadSceneAsync("Level 1");
    }
}
