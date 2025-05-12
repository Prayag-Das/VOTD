using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoAudioScript : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private AudioSource logoAudioSource; // <-- Added AudioSource

    private void Start()
    {
        StartCoroutine(PlayLogoAudio());
    }
    private IEnumerator PlayLogoAudio()
    {
        // Stay black for 0.5 seconds first
        yield return new WaitForSeconds(0.1f);

        // Play logo sound effect
        if (logoAudioSource != null)
        {
            logoAudioSource.Stop();
            logoAudioSource.Play();
        }
    }
}
