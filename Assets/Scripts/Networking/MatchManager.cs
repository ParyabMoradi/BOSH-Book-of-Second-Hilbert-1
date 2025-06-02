using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour
{
    public CameraController cameraController;
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

    [ClientRpc]
    private void SetCameraTargetClientRpc()
    {
        // Each client sets their camera to their own player
        var localClientId = NetworkManager.Singleton.LocalClientId;
        cameraController.SetCameraTarget(PlayerController.AllPlayers[(int)localClientId].transform);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartLevelServerRpc()
    {
        Debug.Log(PlayerController.AllPlayers.Count + " players in the game");

        // Get camera
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();

        foreach (var player in PlayerController.AllPlayers)
        {
            var health = player.GetComponent<PlayerHealth>();
            health.ResetHealth();

            // write a line to reset the player's position
            Debug.Log($"Resetting position for player {player.OwnerClientId}");
            // Reset the player's position
            player.GetComponent<PlayerMovement>().resetPosition = true;
        }

        Debug.Log("Owner client ID: " + OwnerClientId);
        // Remove host-only camera set
        // cameraController.SetCameraTarget(PlayerController.AllPlayers[(int)OwnerClientId].transform);

        // Notify all clients to set their camera target
        SetCameraTargetClientRpc();

        deadCount = 0;
        isGameFrozen.Value = false;

        NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }


}
