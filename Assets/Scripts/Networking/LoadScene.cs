using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneHandler : MonoBehaviour
{
    public void LoadScene(string level)
    {
        SceneManager.LoadScene("Level 1");
    }

    // unload the scene
    public void UnloadScene(string level)
    {
        SceneManager.UnloadSceneAsync("Level 1");
    }
}
