using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class PauseMenu : NetworkBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject runtimeUI;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        if (runtimeUI != null)
        {
            runtimeUI.SetActive(true);
        }

        Debug.Log("PauseMenu initialized. isPaused: " + isPaused);
    }

    void Update()
    {
        if (IsClient && !isPaused && PlayerJoinHandler.IsJoinCanvasActive && Input.GetKeyDown(KeyCode.Escape))
            RequestPauseServerRpc();
        // Only listen to input if this is a client and the join canvas isn't active
        if (!IsClient || PlayerJoinHandler.IsJoinCanvasActive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                RequestResumeServerRpc();
            }
            else
            {
                RequestPauseServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPauseServerRpc(ServerRpcParams rpcParams = default)
    {
        PauseGameClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestResumeServerRpc(ServerRpcParams rpcParams = default)
    {
        ResumeGameClientRpc();
    }

    [ClientRpc]
    private void PauseGameClientRpc()
    {
        if (isPaused)
        {
            Debug.LogWarning("Pause called, but the game is already paused.");
            return;
        }

        pauseMenuUI.SetActive(true);
        if (runtimeUI != null)
        {
            runtimeUI.SetActive(false);
        }

        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game paused. isPaused: " + isPaused);
    }

    [ClientRpc]
    private void ResumeGameClientRpc()
    {
        if (!isPaused)
        {
            Debug.LogWarning("Resume called, but the game is not paused.");
            return;
        }

        pauseMenuUI.SetActive(false);
        if (runtimeUI != null)
        {
            runtimeUI.SetActive(true);
        }

        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game resumed. isPaused: " + isPaused);
    }

    // Called from the Resume button UI
    public void OnResumeButtonPressed()
    {
        if (IsClient && isPaused)
        {
            RequestResumeServerRpc();
        }
    }

    public void GoToScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("QuitGame called. Application is quitting.");
    }
}
