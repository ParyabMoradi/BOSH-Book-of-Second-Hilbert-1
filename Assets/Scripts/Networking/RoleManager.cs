using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;

    private Dictionary<ulong, CharacterType> assignedRoles = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CharacterType GetOrAssignRole(ulong clientId)
    {
        if (assignedRoles.TryGetValue(clientId, out var role))
            return role;

        CharacterType assigned = !assignedRoles.ContainsValue(CharacterType.Boy)
            ? CharacterType.Boy
            : CharacterType.Girl;

        assignedRoles[clientId] = assigned;
        Debug.Log($"[RoleManager] Assigned {assigned} to client {clientId}");
        return assigned;
    }
}
