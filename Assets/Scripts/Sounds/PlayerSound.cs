using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip attackClip;

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpClip);
    }

    public void PlayDashSound()
    {
        audioSource.PlayOneShot(dashClip);
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackClip);
    }
}
