using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Level1Manager : NetworkBehaviour
{
    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(WaitForPlayers());
        }
    }

    private IEnumerator WaitForPlayers()
    {
        while (NetworkManager.Singleton.ConnectedClientsIds.Count < 2)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Debug.Log("[Level1Manager] Both players connected. Level is ready.");
        // You can trigger other game logic here (enemy spawns, intro cutscene, etc.)
    }
}
