using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PlayerController : NetworkBehaviour
{
    public static List<PlayerController> AllPlayers = new List<PlayerController>();


    public NetworkVariable<CharacterType> role = new NetworkVariable<CharacterType>(
        CharacterType.Unassigned,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public GameObject boyModel;
    public GameObject girlModel;


    public Animator boyAnimator;
    public Animator girlAnimator;

    private Animator anim;

    private bool positionSet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AllPlayers.Add(this);
            role.Value = RoleManager.Instance.GetOrAssignRole(OwnerClientId);
        }

        role.OnValueChanged += (_, newValue) =>
        {
            ApplyRole(newValue);
        };

        ApplyRole(role.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            AllPlayers.Remove(this);
        }
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
        boyModel.SetActive(false);
        girlModel.SetActive(false);

        if (r == CharacterType.Boy)
        {
            boyModel.SetActive(true);
            anim = boyAnimator;
        }
        else
        {
            girlModel.SetActive(true);
            anim = girlAnimator;
        }

        Debug.Log($"[CLIENT {OwnerClientId}] Role applied: {r}");
    }

}
