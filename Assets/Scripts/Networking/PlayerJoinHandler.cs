using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerJoinHandler : NetworkBehaviour
{
    [SerializeField] private Canvas joinCanvas;
    [SerializeField] private TextMeshProUGUI codeText;
    private const int MaxPlayers = 2;

    // Static or public flag you can access from other scripts
    public static bool IsJoinCanvasActive { get; private set; }

    private void Start()
    {
        if (!IsServer)
        {
            if (joinCanvas != null)
                joinCanvas.enabled = false;

            IsJoinCanvasActive = false;
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

        ShowJoinUI(true);
        FreezeGame(true);

        if (codeText != null)
            codeText.text = RelayManager.LastJoinCode;
    }

    private void ShowJoinUI(bool show)
    {
        if (joinCanvas != null)
            joinCanvas.enabled = show;

        IsJoinCanvasActive = show;
    }

    private void FreezeGame(bool freeze)
    {
        Time.timeScale = freeze ? 0 : 1;
        AudioListener.pause = freeze;
    }

    private void OnDestroy()
    {
        if (!IsServer || NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            ShowJoinUI(false);
            FreezeGame(false);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count < MaxPlayers)
        {
            ShowJoinUI(true);
            FreezeGame(true);

            if (codeText != null)
                codeText.text = RelayManager.LastJoinCode;
        }
    }
}
