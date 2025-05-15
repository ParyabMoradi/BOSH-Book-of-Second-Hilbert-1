using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkVariable<int> connectedPlayers = new NetworkVariable<int>(0);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        connectedPlayers.Value++;
        Debug.Log($"Player connected: {clientId}. Total: {connectedPlayers.Value}");

        if (connectedPlayers.Value == 2)
        {
            StartGame(); // All players connected
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        connectedPlayers.Value--;
        Debug.Log($"Player disconnected: {clientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        StartGame();
    }

    private void StartGame()
    {
        // Call a function or set a state that all players act on
        Debug.Log("Both players connected. Starting game...");
        // You can trigger synced actions here
    }
}