using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public Slider backgroundMusicSlider;

    public Slider soundFxSlider;
    
    public static AudioManager instance;

    // --------------------------------------------------------------
    [Header("Sound FX")] 
    [Tooltip("Sound after eating fish")]
    public GameObject eatFishSound;

    [Tooltip("Sound after eating plastic")]
    public GameObject eatPlasticSound;

    [Tooltip("Sound when you tap on locked hat cell")]
    public GameObject LockedSound;

    [Tooltip("Sound after selecting hat")]
    public GameObject SelecedHatSound;

    [Tooltip("Sound when you tap on buttons that bring you to another view")]
    public GameObject ClickSound;

    [Tooltip("Sound when you tap on back")]
    public GameObject BackSound;
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

    private void Start()
    {
        var backgroundMusicVolume = PersistencyManager.instance.BackgroundMusicVolume;
        backgroundMusicSlider.value = backgroundMusicVolume;
        BackgroundMusicVolume(backgroundMusicVolume);
        
        var soundFxVolume = PersistencyManager.instance.SoundFxVolume;
        soundFxSlider.value = soundFxVolume;
        SoundFxVolume(soundFxVolume);
    }

    public void Menu() {
        BackgroundMusicManager.instance.Menu();
    }

    public void Play() {
        BackgroundMusicManager.instance.Play();
    }

    public void LevelCompleted() {
        BackgroundMusicManager.instance.LevelCompleted();
    }

    public void Dead() {
        BackgroundMusicManager.instance.Dead();
    }

    public void EatFish() {
        eatFishSound.GetComponent<AudioSource>().Play();
    }

    public void EatPlastic() {
        eatPlasticSound.GetComponent<AudioSource>().Play();
    }

    public void Locked()
    {
        LockedSound.GetComponent<AudioSource>().Play();
    }

    public void SelecedHat()
    {
        SelecedHatSound.GetComponent<AudioSource>().Play();
    }

    public void Click()
    {
        ClickSound.GetComponent<AudioSource>().Play();
    }

    public void backSound()
    {
        BackSound.GetComponent<AudioSource>().Play();
    }
    
    public float MusicVolume()
    {
        masterMixer.GetFloat("Background Music Volume", out var result);

        return result;
    }
    
    public void BackgroundMusicVolume(float sliderValue)
    {
        PersistencyManager.instance.BackgroundMusicVolume = sliderValue;
        setVolume("Background Music Volume", sliderValue);
    }
    
    public void SoundFxVolume(float sliderValue)
    {
        PersistencyManager.instance.SoundFxVolume = sliderValue;
        setVolume("Sound FX Volume", sliderValue);
    }

    private void setVolume(string parameterName, float value)
    {
        if (Math.Abs(value) < 0.001f)
        {
            value = 0.001f;
        }
        
        if (!masterMixer.SetFloat(parameterName, Mathf.Log10(value) * 20))
        {
            Debug.LogError("Mixer setup does not have '" + parameterName + "' exposed");
        }
    }
}
