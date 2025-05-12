using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject JoinPanel;
    public TMP_InputField codeInputField;
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    public void HostLobby()
    {
        // Logic to host a lobby
        Debug.Log("Hosting a lobby...");
        SceneManager.LoadScene("LevelSelect");
    }

    public void JoinLobby()
    {
        LobbyManager.isHost = false;
        LobbyManager.lobbyCode = codeInputField.text.ToUpper();
        SceneManager.LoadScene("LevelSelect");
    }

    public void ShowJoinPanel()
    {
        JoinPanel.SetActive(true);
    }

    public void HideJoinPanel()
    {
        JoinPanel.SetActive(false);
    }
}
