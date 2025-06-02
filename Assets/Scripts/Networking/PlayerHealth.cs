using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 4;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
    4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
    false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);



    public override void OnNetworkSpawn()
    {
        if (IsServer)
    {
        currentHealth.Value -= 1;
        if (currentHealth.Value <= 0)
            isDead.Value = true;
    }
        }

    [ServerRpc(RequireOwnership = false)] 
    public void TakeDamageServerRpc(int amount)
    {
        if (isDead.Value) return;

        currentHealth.Value -= amount;
        //add a debug log to see the current health
        Debug.Log($"Player {OwnerClientId} took damage. Current health: {currentHealth.Value}");

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead.Value = true;
            MatchManager.Instance.OnPlayerDied();
        }
    }

    public void ResetHealth()
    {
        currentHealth.Value = maxHealth;
        isDead.Value = false;
    }
}
