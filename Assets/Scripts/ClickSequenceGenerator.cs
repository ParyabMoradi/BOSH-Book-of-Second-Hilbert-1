using UnityEngine;

public static class ClickSequenceGenerator
{
    // Generates a random sequence of 'L' (left click) and 'R' (right click)
    public static int[] GenerateSequence(int length)
    {
        int[] sequence = new int[length];

        for (int i = 0; i < length; i++)
        {
            sequence[i] = Random.Range(0, 2); // 0 = Left Click, 1 = Right Click

        }
        return sequence;
    }
}