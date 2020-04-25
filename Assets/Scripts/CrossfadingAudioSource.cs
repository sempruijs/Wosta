using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class CrossfadingAudioSource : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Audio")]
    [Tooltip("Clips")]
    public AudioClip[] audioClips;

    [Tooltip("Mixer group")]
    public AudioMixerGroup audioMixerGroup;

    [Tooltip("Use one clip at the time")]
    public bool singleton = true;

    [Tooltip("Audio sources")]
    private AudioSource[] audioSources;

    // --------------------------------------------------------------
    [Header("Volumes")]
    [Tooltip("Target")]
    public float[] targetVolumes;

    [Tooltip("Speed to transition between sources")]
    public float transitionSpeed = 1.0f;

    [Tooltip("Transition complete tolerance")]
    public float transitionComplete = 0.05f;

    [Tooltip("Chance a clip is muted")]
    public float mutePercentage;

    private void Start() {
        audioSources = new AudioSource[audioClips.Length];
        targetVolumes = new float[audioClips.Length];

        for (var i = 0; i < audioClips.Length; i++) {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[i];
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.0f;

            audioSources[i] = audioSource;
            targetVolumes[i] = 0.0f;
        }
    }

    private void Update()
    {
        for (int i = 0; i < audioClips.Length; i++) {
            float targetVolume = targetVolumes[i];
            AudioSource audioSource = audioSources[i];
            float currentVolume = audioSource.volume;

            if (Math.Abs(targetVolume - currentVolume) > 0.0001f) {
                if (targetVolume > 0.0f) {
                    if (!audioSource.isPlaying) {
                        audioSource.time = CurrentTime();
                        audioSource.Play();
                    }
                }
                if (Mathf.Abs(targetVolume - currentVolume) < transitionComplete) {
                    audioSource.volume = targetVolume;

                    if (Math.Abs(targetVolume) < 0.0001f) {
                        audioSource.Stop();
                    }
                } else {
                    audioSource.volume = Mathf.Lerp(currentVolume, targetVolume, transitionSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void Change() {
        var mute = (Random.value < mutePercentage);

        if (mute) {
            for (var i = 0; i < audioClips.Length; i++) {
                targetVolumes[i] = 0.0f;
            }
        } else {
            if (singleton) {
                var track = Random.Range(0, audioClips.Length); // -1 to allow mute

                for (var i = 0; i < audioClips.Length; i++) {
                    var trackEnabled = i == track;
                    targetVolumes[i] = trackEnabled ? 1.0f : 0.0f;
                }
            } else {
                for (var i = 0; i < audioClips.Length; i++) {
                    var trackEnabled = (Random.value > 0.5f);

                    targetVolumes[i] = trackEnabled ? 1.0f : 0.0f;
                }
            }
        }
    }

    public void Stop() {
        for (var i = 0; i < audioClips.Length; i++) {
            targetVolumes[i] = 0.0f;
        }
    }

    public void HalfVolume() {
        for (var i = 0; i < audioClips.Length; i++) {
            targetVolumes[i] = targetVolumes[i] / 4;
        }
    }

    private float CurrentTime() {
        var total = 0.0f;
        var count = 0;

        foreach (var source in GetComponents<AudioSource>()) {
            if (source.isPlaying) {
                count++;
                total += source.time;
            }
        }

        var result = count == 0 ? 0.0f : total / count;

        return result;
    }
}
