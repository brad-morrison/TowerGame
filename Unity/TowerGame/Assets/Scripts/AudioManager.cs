using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")] [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this between scene loads if you want
        // DontDestroyOnLoad(gameObject);

        // Make an AudioSource for SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f; // 2D sounds
        sfxSource.playOnAwake = false;
    }

    public void PlaySfx(AudioClip clip)
    {
        if (!clip) return;
        Debug.Log("Playing SFX");
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySfxWithVolume(AudioClip clip, float volume)
    {
        if (!clip) return;
        Debug.Log("Playing SFX");
        sfxSource.PlayOneShot(clip, volume);
    }

    public void StopSfx(AudioSource src)
    {
        src.Stop();
    }

    public void ActivateSoundSource(AudioSource soundSource)
    {
        soundSource.enabled = true;
    }

    public void PauseSfx(AudioSource src)
    {
        src.Pause();
    }

    public void UnPauseSfx(AudioSource src)
    {
        src.UnPause();
    }

}