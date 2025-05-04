using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Music & Hum")]
    [Tooltip("Looping background music")]
    public AudioSource backgroundMusic;
    [Tooltip("Looping hum sound")]
    public AudioSource humSource;
    [Tooltip("Hum AudioClip")]
    public AudioClip humClip;

    [Header("Intercom & Announcements")]
    [Tooltip("One-shot announcement source")]
    public AudioSource intercomSource;
    [Tooltip("Default warning clip")]
    public AudioClip warningClip;

    [Header("Auto Warning")]
    [Tooltip("Seconds after game start to play the first intercom warning")]
    [SerializeField] private float announcementDelay = 4f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Prepare hum source
        if (humSource != null && humClip != null)
        {
            humSource.clip = humClip;
            humSource.loop = true;
            humSource.playOnAwake = false;
        }

        // Prepare intercom source
        if (intercomSource != null)
            intercomSource.playOnAwake = false;
    }

    private void Start()
    {
        // Background music
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
            backgroundMusic.Play();

        // Intercom warning
        StartCoroutine(AutoWarningCoroutine());
    }

    public void PauseMusic()
    {
        if (backgroundMusic != null)
            backgroundMusic.Pause();
    }

    public void ResumeMusic()
    {
        if (backgroundMusic != null)
            backgroundMusic.UnPause();
    }

    public void PlayHum()
    {
        if (humSource != null && !humSource.isPlaying)
            humSource.Play();
    }

    public void StopHum()
    {
        if (humSource != null && humSource.isPlaying)
            humSource.Stop();
    }

    public void PlayAnnouncement(AudioClip clip)
    {
        if (clip != null && intercomSource != null)
            intercomSource.PlayOneShot(clip);
    }

    private IEnumerator AutoWarningCoroutine()
    {
        yield return new WaitForSeconds(announcementDelay);
        PlayAnnouncement(warningClip);
    }
}
