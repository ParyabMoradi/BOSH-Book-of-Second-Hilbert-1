using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BothClientsInTrigger : MonoBehaviour
{
    public GameObject Wall2;
    private HashSet<ulong> clientsInZone = new HashSet<ulong>();

    [Tooltip("Expected number of clients (players) in zone to trigger action")]
    public int requiredClientCount = 2;

    [Header("Assign These in Inspector")]
    public GameObject door;
    public GameObject wall;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out NetworkObject netObj))
            return;

        if (!netObj.IsPlayerObject)
            return;

        clientsInZone.Add(netObj.OwnerClientId);

        if (!triggered && clientsInZone.Count == requiredClientCount)
        {
            triggered = true;
            Debug.Log("Both clients are inside the trigger!");
            DoSomething();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out NetworkObject netObj))
            return;

        clientsInZone.Remove(netObj.OwnerClientId);
    }

    private void DoSomething()
    {
        if (door != null)
            door.SetActive(true);

        if (wall != null)
            wall.SetActive(true);

        if (Wall2 != null)
            Wall2.SetActive(false);

        Debug.Log("Door enabled, wall disabled.");
    }
}