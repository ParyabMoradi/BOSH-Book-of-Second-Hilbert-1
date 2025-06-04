using UnityEngine;
using UnityEngine.Audio;

public class NewMonoBehaviourScript : MonoBehaviour

{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
    
        // Set the volume in the AudioMixer
        audioMixer.SetFloat("masterVolume", level);
    }
    public void SetMusicVolume(float level)
    {

        // Set the volume in the AudioMixer
        audioMixer.SetFloat("musicVolume", level);
    }
    public void SetSFXVolume(float level)
    {
        

        // Set the volume in the AudioMixer
        audioMixer.SetFloat("soundFXVolume", level);
    }
}
