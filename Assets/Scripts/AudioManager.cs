using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("---------- Audio Source ----------------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("---------- Audio Clip ----------------")]
    public AudioClip background;
    public AudioClip creditScene;
    

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object when loading new scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    private void Start()
    {
        PlayBackground();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

    public void StopBackground()
    {
        musicSource.Stop();
    }

    public void PlayBackground()
    {
        if (musicSource.clip != background)
        {
            musicSource.clip = background;
        }
        musicSource.Play();
    }

    public void PlayCreditScene()
    {
        musicSource.Stop();
        musicSource.clip = creditScene;
        musicSource.Play();
    }
}