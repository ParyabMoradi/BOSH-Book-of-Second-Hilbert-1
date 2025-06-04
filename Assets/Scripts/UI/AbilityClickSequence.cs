using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class AbilityClickSequence : NetworkBehaviour
{
    public Image timeoutBar;

    [Header("Sequence Settings")]
    public int sequenceLength = 6;
    public int maxRepeats = 2;

    [Header("UI Elements")]
    public GameObject leftClickPrefab;    // Red by default
    public GameObject rightClickPrefab;   // Blue by default
    public Transform sequenceContainer;   // Should have Horizontal/Vertical Layout Group
    public Image abilityIndicator;

    private int[] currentSequence;
    private int currentInputIndex = 0;

    [Header("Timeout Settings")]
    public float sequenceTimeout = 5f;

    private float timeRemaining;
    private bool isSequenceTiming = false;
    private CharacterType role;

    private List<GameObject> currentSquares = new List<GameObject>();

    private void Start()
    {
        GenerateAndDisplayNewSequence();
        role = RoleManager.Instance.GetOrAssignRole(NetworkManager.Singleton.LocalClientId);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (isSequenceTiming)
        {
            timeRemaining -= Time.deltaTime;
            timeoutBar.fillAmount = timeRemaining / sequenceTimeout;

            if (timeRemaining <= 0f)
            {
                isSequenceTiming = false;
                currentInputIndex = 0;
                abilityIndicator.color = Color.red;
                Invoke(nameof(GenerateAndDisplayNewSequence), 0.5f);
                timeoutBar.fillAmount = 1;
            }
        }

        if (Input.GetMouseButtonDown(0)) HandleInput(0);
        if (Input.GetMouseButtonDown(1)) HandleInput(1);
    }

    private void HandleInput(int input)
    {
        if (currentSequence == null || currentInputIndex >= currentSequence.Length)
            return;

        if (!isSequenceTiming)
        {
            isSequenceTiming = true;
            timeRemaining = sequenceTimeout;
        }

        if (input == currentSequence[currentInputIndex])
        {
            // Turn completed square green
            Image img = currentSquares[currentInputIndex].GetComponent<Image>();
            img.color = Color.green;

            currentInputIndex++;
            abilityIndicator.color = Color.yellow;

            if (currentInputIndex >= currentSequence.Length)
            {
                Debug.Log("Sequence completed!");
                isSequenceTiming = false;
                abilityIndicator.color = Color.green;
                ClickSequenceHolder.Instance.PopClickSequence(currentSequence);
                TryApplyAbilityToTargetUnderCursor();
                Invoke(nameof(GenerateAndDisplayNewSequence), 0.5f);
                timeoutBar.fillAmount = 1;
            }
        }
        else
        {
            Debug.Log("Wrong input!");
            isSequenceTiming = false;
            currentInputIndex = 0;
            abilityIndicator.color = Color.red;
            timeoutBar.fillAmount = 1;
            Invoke(nameof(GenerateAndDisplayNewSequence), 0.5f);
        }
    }

    private void GenerateAndDisplayNewSequence()
    {
        currentSequence = ClickSequenceHolder.Instance.CreateUniqueClickSequence(sequenceLength, maxRepeats);
        currentInputIndex = 0;
        isSequenceTiming = false;
        timeRemaining = sequenceTimeout;
        abilityIndicator.color = Color.white;

        // Clear previous squares
        foreach (var square in currentSquares)
        {
            Destroy(square);
        }
        currentSquares.Clear();

        // Create new sequence squares
        for (int i = 0; i < currentSequence.Length; i++)
        {
            GameObject prefab = currentSequence[i] == 0 ? leftClickPrefab : rightClickPrefab;
            GameObject square = Instantiate(prefab, sequenceContainer);
            currentSquares.Add(square);
        }
    }

    private void TryApplyAbilityToTargetUnderCursor()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit != null)
        {
            if (role == CharacterType.Boy && hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyShooting>()?.SlowEnemyClientRpc(7f);
                hit.GetComponent<EnemyRandomAreaMover>()?.SlowMovementClientRpc(7f);
                hit.GetComponent<EnemyPathMover>()?.SlowMovementClientRpc(7f);
            }
            else if (role == CharacterType.Girl && hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.ActivateInvulnerabilityServerRpc(7f);
            }
            else
            {
                Debug.Log("Hit object is not a valid target.");
            }
        }
        else
        {
            Debug.Log("No target under cursor.");
        }
    }
}
