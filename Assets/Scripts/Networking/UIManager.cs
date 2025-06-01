using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject hostEndScreen;
    public GameObject clientEndScreen;
    public Button tryAgainButton;

    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowEndScreen(bool isHost)
    {
        //freeze the game when the end screen is shown
        if (isHost)
        {
            hostEndScreen.SetActive(true);
            Time.timeScale = 0;

            tryAgainButton.onClick.RemoveAllListeners();

            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        }
        else
        {
            clientEndScreen.SetActive(true);
                    

        }
    }

    private void OnTryAgainClicked()
    {
        hostEndScreen.SetActive(false);
        Time.timeScale = 1; 
        
        MatchManager.Instance.RestartLevelServerRpc();
    }

    public void HideAllScreens()
    {
        hostEndScreen.SetActive(false);
        clientEndScreen.SetActive(false);
    }
}
