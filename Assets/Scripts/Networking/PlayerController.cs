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
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController boyController;
    public RuntimeAnimatorController girlController;

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
    // Always disable both first
    boyModel.SetActive(false);
    girlModel.SetActive(false);

    // Then enable the correct one
    GameObject activeModel = r == CharacterType.Boy ? boyModel : girlModel;
    activeModel.SetActive(true);

    anim = activeModel.GetComponent<Animator>();
    spriteRenderer = activeModel.GetComponent<SpriteRenderer>();

    if (anim == null || spriteRenderer == null)
        Debug.LogError("Missing Animator or SpriteRenderer on model!");

    Debug.Log($"[CLIENT {OwnerClientId}] Role applied: {r}");
}

}