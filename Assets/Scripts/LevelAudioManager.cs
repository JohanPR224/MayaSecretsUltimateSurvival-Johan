using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelAudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Lee el volumen guardado desde el menú principal
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        audioSource.volume = musicVolume;
    }
}