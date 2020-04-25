using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        slider.value = AudioManager.instance.MusicVolume();
    }

    // Update is called once per frame
    void SliderValueUpdated()
    {
        AudioManager.instance.BackgroundMusicVolume(slider.value);
    }
}
