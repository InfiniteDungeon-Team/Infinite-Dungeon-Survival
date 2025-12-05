using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource music_Source;
    [SerializeField] AudioSource sfx_Source;

    [SerializeField] AudioClip playerShootSFX;

    public void PlayShootSFX()
    {
        sfx_Source.PlayOneShot(playerShootSFX);
    }

    public void StopMusic()
    {
        music_Source.Stop();
    }

    public void Start()
    {
        
    }
}