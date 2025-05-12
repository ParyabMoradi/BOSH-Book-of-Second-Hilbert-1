using System.Collections.Generic;
using UnityEngine;

public class ClickSequenceHolder : MonoBehaviour
{
    public static ClickSequenceHolder Instance { get; private set; }

    private List<int[]> sequenceList = new List<int[]>();
    private Dictionary<string, int> sequenceCountMap = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds a click sequence, even if it's a duplicate.
    /// </summary>
    public void SetClickSequence(int[] sequence)
    {
        string key = SequenceToString(sequence);

        sequenceList.Add(sequence);

        if (sequenceCountMap.ContainsKey(key))
            sequenceCountMap[key]++;
        else
            sequenceCountMap[key] = 1;
        
    }

    /// <summary>
    /// Pops (removes) a single instance of the specified sequence.
    /// </summary>
    public bool PopClickSequence(int[] sequence)
    {
        string key = SequenceToString(sequence);

        for (int i = 0; i < sequenceList.Count; i++)
        {
            if (SequencesEqual(sequenceList[i], sequence))
            {
                sequenceList.RemoveAt(i);

                if (sequenceCountMap.ContainsKey(key))
                {
                    sequenceCountMap[key]--;
                    if (sequenceCountMap[key] <= 0)
                        sequenceCountMap.Remove(key);
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Generates a unique sequence not already stored (based on value, not count).
    /// </summary>
    public int[] CreateUniqueClickSequence(int length, int maxRepeats)
    {
        int[] sequence;
        string key;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            sequence = ClickSequenceGenerator.GenerateSequence(length, maxRepeats);
            key = SequenceToString(sequence);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("Max attempts reached for generating a unique sequence.");
                break;
            }

        } while (sequenceCountMap.ContainsKey(key));

        SetClickSequence(sequence);
        return sequence;
    }

    /// <summary>
    /// Logs all stored sequences.
    /// </summary>
    public void LogAllSequences()
    {
        Debug.Log($"[ClickSequenceHolder] Total Sequences: {sequenceList.Count}");

        foreach (var seq in sequenceList)
        {
            Debug.Log($"Sequence: [{string.Join(", ", seq)}]");
        }

        Debug.Log($"Distinct Sequences: {sequenceCountMap.Count}");
    }

    private string SequenceToString(int[] sequence)
    {
        return string.Join("", sequence);
    }

    private bool SequencesEqual(int[] a, int[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
}
