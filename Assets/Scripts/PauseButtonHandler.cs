using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonHandler : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Buttons")] 
    [Tooltip("Button to toggle pause")]
    public Button pauseButton;
    
    // --------------------------------------------------------------
    [Header("Sprites")] 
    [Tooltip("Pause enabled")]
    public Sprite enabledSprite;
    [Tooltip("Pause disabled")]
    public Sprite disabledSprite;

    // --------------------------------------------------------------
    [Header("Static")] 
    [Tooltip("Games states with enabled pause button")]
    private readonly HashSet<GameManager.State> pauseAvailableStates = new HashSet<GameManager.State>
    {
        GameManager.State.Pause,
        GameManager.State.Playing
    };

    private void OnApplicationPause(bool paused)
    {
        if(paused && GameManager.instance.state == GameManager.State.Playing)
        {
            Toggle();
        }
    }
    
    private void Update()
    {
        UpdateSprite();
        
        if (PauseAvailable())
        {
            HandlePauseKey();
            UpdatePauseButtons();
        }
        else
        {
            DisablePauseButtons();
        }
    }

    private bool PauseAvailable()
    {
        return pauseAvailableStates.Contains(GameManager.instance.state);
    }

    private void UpdatePauseButtons()
    {
        pauseButton.gameObject.SetActive(true);
    }

    private void DisablePauseButtons()
    {
        pauseButton.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (GameManager.instance.state == GameManager.State.Pause)
        {
            GameManager.instance.Play();
        }
        else
        {
            GameManager.instance.Pause();
            DisplayManager.instance.Highlight(DisplayManager.instance.Resume);
        }
    }

    private void HandlePauseKey()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    private bool IsPaused()
    {
        return GameManager.instance.state == GameManager.State.Pause;
    }

    private void UpdateSprite()
    {
        pauseButton.GetComponent<Image>().sprite = IsPaused() ? enabledSprite : disabledSprite;
    }
}