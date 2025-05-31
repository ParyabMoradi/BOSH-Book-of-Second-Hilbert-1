using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneHandler : MonoBehaviour
{
    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }
}
