using UnityEngine;

public class TriggerToDisableOther : MonoBehaviour
{
    [SerializeField] private GameObject objectToDisable;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
    }
}