using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<CharacterType> role = new NetworkVariable<CharacterType>(
        CharacterType.Unassigned,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public GameObject boyModel;
    public GameObject girlModel;
    // public Animator animator;
    // public RuntimeAnimatorController boyController;
    // public RuntimeAnimatorController girlController;



    private bool positionSet = false;

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

    private void Start()
    {
        StartCoroutine(WaitAndSetSpawnPosition());
    }

    private IEnumerator WaitAndSetSpawnPosition()
    {
        while (!IsOwner || role.Value == CharacterType.Unassigned)
        {
            yield return null;
        }

        if (!positionSet)
        {
            SetSpawnPosition();
            positionSet = true;
        }
    }

    private void SetSpawnPosition()
    {
        string spawnName = role.Value == CharacterType.Boy ? "SpawnPoint_Boy" : "SpawnPoint_Girl";
        GameObject spawnPoint = GameObject.Find(spawnName);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            Debug.Log($"[CLIENT {OwnerClientId}] Spawned at {spawnName}");
        }
        else
        {
            Debug.LogWarning($"[CLIENT {OwnerClientId}] Could not find {spawnName}");
        }
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

        // if (animator != null)
        // {
        //     animator.runtimeAnimatorController = r == CharacterType.Boy ? boyController : girlController;
        // }

        Debug.Log($"[CLIENT {OwnerClientId}] Role applied: {r}");
    }
}
