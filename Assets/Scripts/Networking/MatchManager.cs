using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance;

    private void Awake() => Instance = this;

    private int deadCount = 0;

    public NetworkVariable<bool> isGameFrozen = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public void OnPlayerDied()
    {
        deadCount++;
        isGameFrozen.Value = true;
        ShowFailureScreenClientRpc();
    }

    [ClientRpc]
    private void ShowFailureScreenClientRpc()
    {
        bool isHost = NetworkManager.Singleton.IsHost;
        UIManager.Instance.ShowEndScreen(isHost);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartLevelServerRpc()
    {
        Debug.Log(PlayerController.AllPlayers.Count + " players in the game");
        foreach (var player in PlayerController.AllPlayers)
        {
            var health = player.GetComponent<PlayerHealth>();
            health.ResetHealth();

            // write a line to reset the player's position
            Debug.Log($"Resetting position for player {player.OwnerClientId}");
            // Reset the player's position
            player.GetComponent<PlayerMovement>().resetPosition = true;



        }

        deadCount = 0;
        isGameFrozen.Value = false;



     
        NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }
        
    
}
