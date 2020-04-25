using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerButtonHandler : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Buttons")] 
    [Tooltip("Button to toggle accelerometer")]
    public Button accelerometerButton;

    // --------------------------------------------------------------
    [Header("Sprites")] 
    [Tooltip("Accelerometer enabled")]
    public Sprite enabledSprite;
    [Tooltip("Accelerometer disabled")]
    public Sprite disabledSprite;

    // --------------------------------------------------------------
    [Header("Static")] 
    [Tooltip("Games states with enabled accelerometer button")]
    private readonly HashSet<GameManager.State> accelerometerAvailableStates = new HashSet<GameManager.State>
    {
        GameManager.State.Pause,
        GameManager.State.Playing
    };

    private void Start()
    {
        UpdateSprite();
    }
    
    private void Update()
    {
        accelerometerButton.gameObject.SetActive(AccelerometerAvailable());
    }

    private bool AccelerometerAvailable()
    {
        return accelerometerAvailableStates.Contains(GameManager.instance.state) &&
               SystemInfo.supportsAccelerometer;
    }

    public void Toggle()
    {
        PersistencyManager.instance.Accelerometer = !PersistencyManager.instance.Accelerometer;

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        accelerometerButton.GetComponent<Image>().sprite = 
            PersistencyManager.instance.Accelerometer ? enabledSprite : disabledSprite;
    }
}
