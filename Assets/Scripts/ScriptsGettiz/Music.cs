using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    void Start()
    {
 
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PlayTheSound();
    }

    public void PlayTheSound()
    {
        audioSource.PlayOneShot(clip);
    }
}
