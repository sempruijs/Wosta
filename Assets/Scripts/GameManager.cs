using System;
using UnityEngine;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Singleton")] 
    [Tooltip("Singleton instance")]
    public static GameManager instance;

    // --------------------------------------------------------------
    [Header("Player")] 
    [Tooltip("Player of the game")]
    public GameObject player;

    [Tooltip("Initial speed (at the start of new level)")]
    public float initialSpeed = 3;
    
    [Tooltip("Position staring new level")]
    public Vector3 initialPosition;

    [Tooltip("Size staring new level")]
    public float normalSize = 0.05f;

    [Tooltip("Size staring new level")]
    public float maximumSize = 0.5f;

    public Vector3 Zposition; 

    public Vector3 dayMode;
    public Vector3 nightMode;   
    // --------------------------------------------------------------
    [Header("Display")] 
    [Tooltip("Width of the game")]
    public float width = 16;

    [Tooltip("Height of the game")]
    public float height = 9;

    public GameObject dayBackground;
    public GameObject nightBackground;

    [Tooltip("Width of the game")] 
    public GameCenterManager gameCenterManager;
    
    // --------------------------------------------------------------
    [Header("Progress")] 
    [Tooltip("Number of points")]
    public int score;
    [Tooltip("Level")]
    public int level;
    public GameObject ScorePrefab;   
    public float scoreForFish;
    // --------------------------------------------------------------
    [Header("Game Rules")]
    [Tooltip("Speed impact of touching plastic")]
    public float plasticHitSpeedImpact = 0.6f;

    [Tooltip("Percentage of enemy that is added to player after eating")]
    public float enemyGrowFactor = 0.1f;

    [Tooltip("Maximum points per enemy")]
    public int maximumPointsPerEnemy = 1000;

    [Tooltip("Initial level (should be 1, but can be used to test higher levels)")] 
    public int initialLevel = 1;    

    [Tooltip("Number of levels")] 
    public int numberOfLevels = 100;    

    // --------------------------------------------------------------
    [Header("Cache")] 
    [Tooltip("Player controller")]
    public PlayerController playerController;

    // --------------------------------------------------------------
    [Header("State")] 
    [Tooltip("State")]
    public State state;

    public State stateBeforeSettings;

    public enum State { Menu, Loading, Playing, NextLevel, Dead, CompletedGame, Pause, Settings, Credit, Fish, HighScore, Tutorial}

    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        InitGame();
    }

    public void Settings()
    {
        if (state == State.Menu || state == State.Pause)
        {
            stateBeforeSettings = state;
        }
        state = State.Settings;
        DisplayManager.instance.UpdateUi();
    }

    public void credit() {
        state = State.Credit;
        DisplayManager.instance.UpdateUi();
    }

    public void CloseSettings()
    {
        state = stateBeforeSettings;
        DisplayManager.instance.UpdateUi();
    }

    private void InitGame()
    {
        state = State.Menu;
        ResetScore();
        DisplayManager.instance.UpdateUi();
    }

    private void Start()
    {
        Application.targetFrameRate = 300;
        
        playerController = player.GetComponent<PlayerController>();
        DisplayManager.instance.UpdateUi();
        Menu();
    }


    private void Update()
    {
        if (state == State.Playing && Input.GetAxis("Options") == 1)
        {
            Pause();
        }
    }

    public void Menu() {
        state = State.Menu;
        stateBeforeSettings = State.Menu;
        AudioManager.instance.Menu();
        DisplayManager.instance.UpdateUi();
        gameCenterManager.PostScoreOnLeaderBoard(PersistencyManager.instance.HighScore);
    }

    public void Fish()
    {
        state = State.Fish;
        DisplayManager.instance.UpdateUi();
    }

    public void HighScore()
    {
        state = State.HighScore;
        DisplayManager.instance.UpdateUi();
    }

    public void Tutorial()
    {
        state = State.Tutorial;
        DisplayManager.instance.UpdateUi();
    }

    public void Play()
    {
        switch (state)
        {
            case State.Menu:
                ResetPlayer();
                ResetScore();
                ToggleTime();
                LoadLevel();
                AudioManager.instance.Play();
                break;
            case State.CompletedGame:
            case State.Dead:
                ResetScore();
                ToggleTime();
                LoadLevel();
                AudioManager.instance.Play();
                break;
            case State.NextLevel:
                state = State.Loading;
                level++;
                Hats.instance.UnlockHatsWhenNextLevel();
                ToggleTime();
                LoadLevel();
                AudioManager.instance.Play();
                break;
            case State.Pause:
                state = State.Playing;
                break;
        }
        DisplayManager.instance.UpdateUi();
    }

    public bool IsPaused()
    {
        DisplayManager.instance.UpdateUi();
        return state == State.Pause || state == State.Settings && stateBeforeSettings == State.Pause;
    }

    public void Pause()
    {
        if (state == State.Playing)
        {
            state = State.Pause;
        }
        DisplayManager.instance.Highlight(DisplayManager.instance.Resume);
        DisplayManager.instance.UpdateUi();
    }
    
    public void ToggleTime() {
        if (level % 2 == 1) {
            print("day");
            dayBackground.transform.position = dayMode;
            nightBackground.transform.position = nightMode;
        } else {
            print("night");
            dayBackground.transform.position = nightMode;
            nightBackground.transform.position = dayMode;
        }
    }
    public void EatEnemy(GameObject subject, GameObject enemy)
    {
        if (state == State.Playing)
        {
            if (PlayerIsBiggerThanEnemy(subject, enemy))
            {
               IncreaseScore(CalculateScore(subject, enemy));
                Grow(enemy);
                Destroy(enemy);
                AudioManager.instance.EatFish();
                Instantiate(ScorePrefab, new Vector3(subject.transform.position.x, subject.transform.position.y, subject.transform.position.z), Quaternion.identity);
               
                if (CompletedLevel())
                {
                    if (level == numberOfLevels)
                    {
                        CompletedGame();
                    }
                    else
                    {
                        NextLevel();
                    }
                }
            }
            else
            {
                state = State.Dead;
                DisplayManager.instance.Highlight(DisplayManager.instance.Home);
                player.GetComponent<PlayerController>().Become(enemy);
                Destroy(enemy);
                gameCenterManager.PostScoreOnLeaderBoard(PersistencyManager.instance.HighScore);
                AudioManager.instance.Dead();
                DisplayManager.instance.UpdateUi();
            }
        }
    }

    private int CalculateScore(GameObject subject, GameObject enemy)
    {
        var subjectSize = subject.transform.localScale.magnitude;
        var enemySize = enemy.transform.localScale.magnitude;

        scoreForFish = 0.0f;
        scoreForFish = enemySize / subjectSize * maximumPointsPerEnemy;

        return (int)(enemySize / subjectSize * maximumPointsPerEnemy);
    }

    private static bool PlayerIsBiggerThanEnemy(GameObject player, GameObject enemy)
    {
        return player.transform.localScale.magnitude > enemy.transform.localScale.magnitude;
    }

    private void Grow(GameObject subject)
    {
        // Read scale of enemy
        var enemyScale = subject.transform.localScale;

        // Determine the scale to add to the player
        var growth = new Vector3(
            enemyScale.x * enemyGrowFactor,
            enemyScale.y * enemyGrowFactor,
            enemyScale.z * enemyGrowFactor);

        // Add scale to player
        player.transform.localScale += growth;
    }

    private bool CompletedLevel()
    {
        return player.transform.localScale.x > maximumSize;
    }

    public void EatPlastic(GameObject subject, GameObject plastic)
    {
        if (state == State.Playing)
        {
            AudioManager.instance.EatPlastic();
            Destroy(plastic);
            subject.GetComponent<PlayerController>().speed *= plasticHitSpeedImpact;
        }
    }

    private void IncreaseScore(int amount)
    {
        score += amount;

        if (score > PersistencyManager.instance.HighScore)
        {
            PersistencyManager.instance.HighScore = score;
        }
    }

    private void AddWosCoin(int amount)
    {
        PersistencyManager.instance.WosCoins = PersistencyManager.instance.WosCoins + amount;
    }

    private void LoadLevel()
    {
        state = State.Loading;
        DisplayManager.instance.UpdateUi();
        CameraManager.instance.Transition(PrepareNextLevel, LoadingComplete);
    }

    private void PrepareNextLevel()
    {
        ResetPlayer();
        LevelManager.instance.CreateLevel();
    }
    
    private void LoadingComplete()
    {
        state = State.Playing;
        DisplayManager.instance.UpdateUi();
    }

    private void NextLevel()
    {
        if (state == State.Playing)
        {
            state = State.NextLevel;
            AddWosCoin(level);
            AudioManager.instance.LevelCompleted();
            gameCenterManager.PostScoreOnLeaderBoard(score);
            DisplayManager.instance.Highlight(DisplayManager.instance.NextLevel);
            DisplayManager.instance.UpdateUi();
        }
    }

    private void CompletedGame()
    {
        if (state == State.Playing)
        {
            state = State.CompletedGame;
            DisplayManager.instance.UpdateUi();
            AudioManager.instance.LevelCompleted();
            gameCenterManager.PostScoreOnLeaderBoard(score);
        }
    }
    
    private void ResetScore()
    {
        score = 0;
        level = initialLevel;
    }

    private void ResetPlayer()
    {
        player.transform.localScale = new Vector3(normalSize, normalSize, normalSize);
        player.transform.position = initialPosition;
        playerController.speed = initialSpeed;
        playerController.velocity = Vector2.zero;
    }

    public float Progress()
    {
        return (float)level / numberOfLevels;
    }
}
