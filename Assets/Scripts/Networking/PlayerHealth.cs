using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 4;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float invulnerabilityEndTime;
    private Coroutine invulnerabilityCoroutine;

    public AudioClip TakeDamageSFX; // Sound effect for taking damage

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }

        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int amount)
    {
        if (isDead.Value || Time.time < invulnerabilityEndTime) return;

        currentHealth.Value -= amount;
        currentHealth.Value = Mathf.Max(currentHealth.Value, 0);

        Debug.Log($"Player {OwnerClientId} took damage. Current health: {currentHealth.Value}");

        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);

        if (currentHealth.Value <= 0 && !isDead.Value)
        {
            isDead.Value = true;
            MatchManager.Instance?.OnPlayerDied();
        }

        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            AudioManager.Instance.PlaySFX(TakeDamageSFX); // Play damage sound
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetHealthServerRpc()
    {
        if (!IsServer) return;

        currentHealth.Value = maxHealth;
        isDead.Value = false;
        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateInvulnerabilityServerRpc(float duration)
    {
        float newEndTime = Time.time + duration;
        invulnerabilityEndTime = Mathf.Max(invulnerabilityEndTime, newEndTime); // extend if needed

        if (invulnerabilityCoroutine == null)
        {
            invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine());
        }

        EnableInvulnerabilityClientRpc();
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        while (Time.time < invulnerabilityEndTime)
        {
            yield return null;
        }

        DisableInvulnerabilityClientRpc();
        invulnerabilityCoroutine = null;
    }

    [ClientRpc]
    private void EnableInvulnerabilityClientRpc()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = true;
    }

    [ClientRpc]
    private void DisableInvulnerabilityClientRpc()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;
    }

    [ClientRpc]
    private void UpdateHeartUIClientRpc(int health, ulong targetClientId)
    {
        if (HeartContainerUI.Instance != null)
        {
            HeartContainerUI.Instance.SetHeartCount((int)targetClientId, health);
        }
    }

    [ClientRpc]
    public void KillPlayerClientRpc()
    {
        // if (!IsServer) return;
        if (isDead.Value) return;
        isDead.Value = true;
        MatchManager.Instance?.OnPlayerDied();
    }
}
