using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // If you're using TextMeshPro

public class LobbyManager : MonoBehaviour
{
    public TMP_Text lobbyCodeText;
    public TMP_Text playerListText;
    public GameObject startButton;

    public static string lobbyCode = ""; // Shared across scenes
    public static bool isHost = false;

    void Start()
    {
        if (isHost)
        {
            lobbyCode = GenerateCode();
        }

        lobbyCodeText.text = "Lobby Code: " + lobbyCode;
        UpdatePlayerList();
        startButton.SetActive(isHost);
    }

    void UpdatePlayerList()
    {
        // Dummy data for now
        if (isHost)
            playerListText.text = "Player1 (Host)\nPlayer2";
        else
            playerListText.text = "Player1 (Host)\nYou";
    }

    string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < 6; i++)
        {
            code += chars[Random.Range(0, chars.Length)];
        }
        return code;
    }

    public void StartGame()
    {
        if (isHost)
        {
            SceneManager.LoadScene("Level1"); // Change as needed
        }
    }

    public void LeaveLobby()
    {
        isHost = false;
        SceneManager.LoadScene("MainMenu");
    }
}
