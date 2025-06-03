using UnityEngine;

public class HeartContainerUI : MonoBehaviour
{
    public static HeartContainerUI Instance;

    public Transform[] heartSets; // Indexed by ClientId (0 and 1 for 2 players)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetHeartCount(int clientId, int count)
    {
        if (clientId >= heartSets.Length)
        {
            Debug.LogWarning($"No heart set for ClientId {clientId}");
            return;
        }

        Transform container = heartSets[clientId];

        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).gameObject.SetActive(i < count);
        }
    }
}