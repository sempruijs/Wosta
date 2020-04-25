using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Singleton")]
    [Tooltip("Singleton instance")]
    public static BackgroundMusicManager instance;

    // --------------------------------------------------------------
    [Header("Music")]
    [Tooltip("In game music")]
    public GameObject inGameMusic;

    [Tooltip("Music after dying")]
    public GameObject deathMusic;

    [Tooltip("Music in menu")]
    public GameObject menuMusic;

    [Tooltip("Startup sound")]
    public GameObject whyHeIsSayingUghMusic;

    [Header("Configuration")] [Tooltip("Time (seconds) after which the in-game music changes")]
    public float inGameMusicChangeRate = 60.0f;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);    
        }

        DontDestroyOnLoad(gameObject);
    }
    
    private void Stop() {
        deathMusic.GetComponent<AudioSource> ().Stop();
        menuMusic.GetComponent<AudioSource> ().Stop();
        CancelInvoke(nameof(ChangeInGameMusic));
        foreach (CrossfadingAudioSource source in inGameMusic.GetComponents<CrossfadingAudioSource>()) {
            source.Stop();
        }
    }
    public void Play() {
        Stop();

        InvokeRepeating(nameof(ChangeInGameMusic), 0, inGameMusicChangeRate);
    }

    private void ChangeInGameMusic()
    {
        foreach (CrossfadingAudioSource source in inGameMusic.GetComponents<CrossfadingAudioSource>()) {
            source.Change();
        }
    }
    
    public void LevelCompleted() {
        foreach (CrossfadingAudioSource source in inGameMusic.GetComponents<CrossfadingAudioSource>()) {
            source.HalfVolume();
        }
        whyHeIsSayingUghMusic.GetComponent<AudioSource> ().Play();
    }

    public void Dead() {
        Stop();
        deathMusic.GetComponent<AudioSource> ().Play();
    }

    public void Menu() {
        Stop();
        menuMusic.GetComponent<AudioSource> ().Play();
        whyHeIsSayingUghMusic.GetComponent<AudioSource> ().Play();
    }
}
