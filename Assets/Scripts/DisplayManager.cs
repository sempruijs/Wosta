using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager instance;

    public TMP_Text score;
    public TMP_Text level;
    public TMP_Text highScore;
    public TMP_Text highScoreInMenu;
    public TMP_Text WosCoins;
    
    public GameObject inGameHud;
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject nextLevelMenu;
    public GameObject completedGameMenu;
    public GameObject deadMenu;
    public GameObject settingsMenu;
    public GameObject creditMenu;
    public GameObject house;
    public GameObject crab;
    public GameObject player;
    public GameObject FishMenu;
    public GameObject WosCoinHatMenu;
    public GameObject NormalHatMenu;
    public GameObject HighScoreMenu;
    public GameObject TutorialMenu;
    
    //for controler navigation
    public GameObject Resume;
    public GameObject NextLevel;
    public GameObject Home;
    public GameObject PlayButton;
    
    private readonly HashSet<GameManager.State> inGameStates = new HashSet<GameManager.State>()
    {
        GameManager.State.Loading,
        GameManager.State.Dead,
        GameManager.State.Pause,
        GameManager.State.Playing,
        GameManager.State.NextLevel
    };
    
    void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);    
        }

        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if (GameManager.instance.state == GameManager.State.Playing)
        {
            if (Input.GetAxis("Options") == 1)
            {
                GameManager.instance.Pause();
                Hilight(Resume);
            }
        }
        if (GameManager.instance.state == GameManager.State.NextLevel) {
                Hilight(NextLevel);
        }
    }

    public void UpdateUi()
    {
        inGameHud.SetActive(inGameStates.Contains(GameManager.instance.state));
        mainMenu.SetActive(GameManager.instance.state == GameManager.State.Menu);
        pauseMenu.SetActive(GameManager.instance.state == GameManager.State.Pause);
        nextLevelMenu.SetActive(GameManager.instance.state == GameManager.State.NextLevel);
        completedGameMenu.SetActive(GameManager.instance.state == GameManager.State.CompletedGame);
        deadMenu.SetActive((GameManager.instance.state == GameManager.State.Dead));
        settingsMenu.SetActive(GameManager.instance.state == GameManager.State.Settings);
        creditMenu.SetActive(GameManager.instance.state == GameManager.State.Credit);
        house.SetActive(GameManager.instance.state == GameManager.State.Playing || GameManager.instance.state == GameManager.State.Dead || GameManager.instance.state == GameManager.State.Pause);
        crab.SetActive(GameManager.instance.state == GameManager.State.Playing || GameManager.instance.state == GameManager.State.Dead || GameManager.instance.state == GameManager.State.Pause);
        player.SetActive(GameManager.instance.state == GameManager.State.Playing || GameManager.instance.state == GameManager.State.NextLevel || GameManager.instance.state == GameManager.State.Loading || GameManager.instance.state == GameManager.State.Dead || GameManager.instance.state == GameManager.State.Pause || GameManager.instance.state == GameManager.State.Fish); 
        FishMenu.SetActive(GameManager.instance.state == GameManager.State.Fish);
        HighScoreMenu.SetActive(GameManager.instance.state == GameManager.State.HighScore);
        TutorialMenu.SetActive(GameManager.instance.state == GameManager.State.Tutorial);
        score.text = GameManager.instance.score.ToString();
        level.text = GameManager.instance.level.ToString();   
        highScore.text = PersistencyManager.instance.HighScore.ToString();
        highScoreInMenu.text = PersistencyManager.instance.HighScore.ToString() + " !";
        WosCoins.text = PersistencyManager.instance.WosCoins.ToString();
    }

    public void ChangeHat(string to)
    {
        if (to == "normal")
        {
            WosCoinHatMenu.SetActive(false);
            NormalHatMenu.SetActive(true);
        } else if (to == "WosCoin")
        {
             WosCoinHatMenu.SetActive(true);
             NormalHatMenu.SetActive(false);           
        }
    }
    
    public void Hilight(GameObject button)
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(button, null);
    }

    public void awayFromSettings()
    {
        if (GameManager.instance.stateBeforeSettings == GameManager.State.Pause)
        {
            Hilight(Resume);
        }
        else if (GameManager.instance.stateBeforeSettings == GameManager.State.Menu)
        {
             Hilight(PlayButton);           
        }
    }
}
