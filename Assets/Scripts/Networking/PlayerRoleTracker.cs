using System.Collections.Generic;
using UnityEngine;

public class PlayerRoleTracker : MonoBehaviour
{
    public static PlayerRoleTracker Instance;
    private Dictionary<ulong, CharacterType> assignedRoles = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public CharacterType GetOrAssignRole(ulong clientId)
    {
        if (assignedRoles.ContainsKey(clientId))
            return assignedRoles[clientId];

        // Assign host as Boy, next player as Girl
        CharacterType assigned = assignedRoles.ContainsValue(CharacterType.Boy) ? CharacterType.Girl : CharacterType.Boy;
        assignedRoles[clientId] = assigned;

        Debug.Log($"[PlayerRoleTracker] Assigned {assigned} to client {clientId}");
        return assigned;
    }
}
