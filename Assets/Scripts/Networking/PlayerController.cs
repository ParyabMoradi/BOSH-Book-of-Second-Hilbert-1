using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<CharacterType> role = new NetworkVariable<CharacterType>(
        CharacterType.Unassigned,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public GameObject boyModel;
    public GameObject girlModel;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            role.Value = RoleManager.Instance.GetOrAssignRole(OwnerClientId);
        }

        role.OnValueChanged += (_, newValue) =>
        {
            ApplyRole(newValue);
        };

        ApplyRole(role.Value);
    }

    private void ApplyRole(CharacterType r)
    {
        if (boyModel == null || girlModel == null)
        {
            Debug.LogError("PlayerController: boyModel or girlModel not assigned.");
            return;
        }

        boyModel.SetActive(r == CharacterType.Boy);
        girlModel.SetActive(r == CharacterType.Girl);
        Debug.Log($"[CLIENT {OwnerClientId}] I am {r}");
    }
}
