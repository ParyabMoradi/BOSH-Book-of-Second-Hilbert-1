using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerEnemyManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyManager enemyManager;
    public ulong playerId;

    private IEnumerator Start()
    {
        // Wait for the network session to start
        yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost);

        // Wait for EnemyManager to exist and be spawned
        EnemyManager enemyManager = null;
        yield return new WaitUntil(() =>
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            return enemyManager != null && enemyManager.IsSpawned;
        });
        if (IsOwner)
        {
        enemyManager.RegisterPlayerServerRpc(gameObject);
            playerId = NetworkManager.Singleton.LocalClientId;
        }
        else
        {
            playerId = NetworkObject.OwnerClientId;
        }
    }

}
