using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityClickSequence : MonoBehaviour
{
    [Header("Sequence Settings")]
    public int sequenceLength = 6;
    public int maxRepeats = 2;

    [Header("UI Elements")]
    public TextMeshProUGUI sequenceText;
    public Image abilityIndicator; // Assign in Inspector

    private int[] currentSequence;
    private int currentInputIndex = 0;

    private void Start()
    {
        GenerateAndDisplayNewSequence();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(0);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleInput(1);
        }
    }

    private void HandleInput(int input)
    {
        if (currentSequence == null || currentInputIndex >= currentSequence.Length)
            return;

        if (input == currentSequence[currentInputIndex])
        {
            currentInputIndex++;
            UpdateSequenceDisplay();

            // Optional: briefly change color on progress
            abilityIndicator.color = Color.yellow;

            if (currentInputIndex >= currentSequence.Length)
            {
                Debug.Log("Sequence completed!");

                abilityIndicator.color = Color.green; // success
                ClickSequenceHolder.Instance.PopClickSequence(currentSequence);
                Invoke(nameof(GenerateAndDisplayNewSequence), 0.5f);
            }
        }
        else
        {
            Debug.Log("Wrong input!");
            currentInputIndex = 0;
            UpdateSequenceDisplay();
            abilityIndicator.color = Color.red; // failure
        }
    }

    private void GenerateAndDisplayNewSequence()
    {
        currentSequence = ClickSequenceHolder.Instance.CreateUniqueClickSequence(sequenceLength, maxRepeats);
        currentInputIndex = 0;
        abilityIndicator.color = Color.white; // reset color
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
}
