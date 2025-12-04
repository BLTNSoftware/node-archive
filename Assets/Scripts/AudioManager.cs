using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip flipClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip mismatchClip;
    [SerializeField] private AudioClip gameOverClip;

    private AudioSource audioSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    public void PlayFlip()
    {
        PlayOneShot(flipClip);
    }

    public void PlayMatch()
    {
        PlayOneShot(matchClip);
    }

    public void PlayMismatch()
    {
        PlayOneShot(mismatchClip);
    }

    public void PlayGameOver()
    {
        PlayOneShot(gameOverClip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
