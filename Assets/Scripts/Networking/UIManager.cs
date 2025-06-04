using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
            Time.timeScale = 0;

            clientEndScreen.SetActive(true);
                    

        }
    }
    
    public GameObject loadingScreen;

    public void ShowLoadingScreen()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }


    private void OnTryAgainClicked()
    {
        hostEndScreen.SetActive(false);
        Time.timeScale = 1;

        MatchManager.Instance.RestartLevelClientRpc(SceneManager.GetActiveScene().name);
    }

    public void HideAllScreens()
    {
        hostEndScreen.SetActive(false);
        Time.timeScale = 1;
        clientEndScreen.SetActive(false);

    }
}
