using UnityEngine;

public class BuildingGlowTrigger2D : MonoBehaviour
{
    public GameObject GlowFrame;

    void Start()
    {
        GlowFrame.SetActive(false); // Hide glow by default
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GlowFrame.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GlowFrame.SetActive(false);
        }
    }
}
