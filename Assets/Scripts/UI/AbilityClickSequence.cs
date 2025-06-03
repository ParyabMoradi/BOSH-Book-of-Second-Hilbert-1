using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityClickSequence : MonoBehaviour
{
    
    public Image timeoutBar;
    [Header("Sequence Settings")]
    public int sequenceLength = 6;
    public int maxRepeats = 2;

    [Header("UI Elements")]
    public TextMeshProUGUI sequenceText;
    public Image abilityIndicator; // Assign in Inspector

    private int[] currentSequence;
    private int currentInputIndex = 0;
    
    [Header("Timeout Settings")]
    public float sequenceTimeout = 5f;

    private float timeRemaining;
    private bool isSequenceTiming = false;



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

            // Update the fillAmount of the ability indicator (image bar)
            timeoutBar.fillAmount = timeRemaining / sequenceTimeout;

            if (timeRemaining <= 0f)
            {
                isSequenceTiming = false;
                currentInputIndex = 0;
                abilityIndicator.color = Color.red; // failure
                UpdateSequenceDisplay();
                Invoke(nameof(GenerateAndDisplayNewSequence), 0.5f); // retry after timeout
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

        // Start the timer when the player first inputs correctly
        if (!isSequenceTiming)
        {
            isSequenceTiming = true;
            timeRemaining = sequenceTimeout; // Start the timer
        }

        if (input == currentSequence[currentInputIndex])
        {
            currentInputIndex++;
            UpdateSequenceDisplay();
            abilityIndicator.color = Color.yellow; // optional: highlight current input

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
            UpdateSequenceDisplay();
            abilityIndicator.color = Color.red;
            timeoutBar.fillAmount = 1;
        }
    }


    private void GenerateAndDisplayNewSequence()
    {
        currentSequence = ClickSequenceHolder.Instance.CreateUniqueClickSequence(sequenceLength, maxRepeats);
        currentInputIndex = 0;
        isSequenceTiming = false; // Stop timing if needed
        timeRemaining = sequenceTimeout; // Reset timer for new sequence
        abilityIndicator.color = Color.white; // Reset indicator color
        UpdateSequenceDisplay();
    }



    private void UpdateSequenceDisplay()
    {
        List<string> elements = new List<string>();

        for (int i = 0; i < currentSequence.Length; i++)
        {
            if (i < currentInputIndex)
            {
                // Completed part: highlight in green
                elements.Add($"<color=green>{currentSequence[i]}</color>");
            }
            else if (i == currentInputIndex)
            {
                // Current target: yellow
                elements.Add($"<color=yellow>{currentSequence[i]}</color>");
            }
            else
            {
                // Not yet completed
                elements.Add(currentSequence[i].ToString());
            }
        }

        sequenceText.text = string.Join(" ", elements);
    }
    
    private void TryApplyAbilityToTargetUnderCursor()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Only hit colliders on a specific layer if desired (e.g., LayerMask.GetMask("Player"))
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit != null)
        {
            if (role == CharacterType.Boy && hit.CompareTag("Enemy"))
            {
                Debug.Log("here");
                hit.GetComponent<EnemyShooting>()?.SlowEnemyClientRpc(7f);
                hit.GetComponent<EnemyRandomAreaMover>()?.SlowMovementClientRpc(7f);
                hit.GetComponent<EnemyPathMover>()?.SlowMovementClientRpc(7f);

                // Optionally call a method on that object
                // hit.GetComponent<PlayerAbilityTarget>()?.OnAbilityHit(); // Your custom method
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
