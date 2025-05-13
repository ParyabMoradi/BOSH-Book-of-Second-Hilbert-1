using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject JoinPanel;
    public TMP_InputField codeInputField;
    public LobbyManagerNetwork lobbyNetwork;


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
        PlayerPrefs.SetInt("isHost", 1); // Save flag to use after scene loads
        SceneManager.LoadScene("LevelSelect");
    }

    public void JoinLobby()
    {
        PlayerPrefs.SetInt("isHost", 0);
        PlayerPrefs.SetString("joinCode", codeInputField.text.ToUpper());
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
