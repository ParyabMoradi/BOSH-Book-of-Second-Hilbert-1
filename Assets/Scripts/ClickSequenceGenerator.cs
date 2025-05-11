using UnityEngine;

public static class ClickSequenceGenerator
{
    
    public static int[] GenerateSequence(int length, int maxRepeats)
    {
        int[] sequence = new int[length];
        int lastValue = -1;
        int repeatCount = 0;

        for (int i = 0; i < length; i++)
        {
            int nextValue;

            if (repeatCount >= maxRepeats)
            {
                nextValue = 1 - lastValue;
                repeatCount = 1;
            }
            else
            {
                nextValue = Random.Range(0, 2);

                if (nextValue == lastValue)
                {
                    repeatCount++;
                }
                else
                {
                    repeatCount = 1;
                }
            }

            sequence[i] = nextValue;
            lastValue = nextValue;
        }

        return sequence;
    }
}