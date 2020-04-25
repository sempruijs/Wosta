using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistencyManager : MonoBehaviour
{
    public static PersistencyManager instance;

    private static string HighScoreKey = "HighScore";
    private static string AccelerometerKey = "Accelerometer";
    private static string BackgroundMusicVolumeKey = "BackgroundMusicVolume";
    private static string SoundFxVolumeKey = "SoundFxVolume";
    private static string WosCoinsKey = "WosCoins";
    private static string NormalHatIdsKey = "NormalHatIds";
    public int HighScore
    {
        get => PlayerPrefs.GetInt(HighScoreKey, 0);
        set => PlayerPrefs.SetInt(HighScoreKey, value);
    }

    public int WosCoins
    {
        get => PlayerPrefs.GetInt(WosCoinsKey, 0);
        set => PlayerPrefs.SetInt(WosCoinsKey, value);
    }

    public bool Accelerometer
    {
        get => PlayerPrefs.GetInt(AccelerometerKey, 0) != 0;
        set => PlayerPrefs.SetInt(AccelerometerKey, value ? 1 : 0);
    }

    public float BackgroundMusicVolume
    {
        get => PlayerPrefs.GetFloat(BackgroundMusicVolumeKey, 1.0f);
        set => PlayerPrefs.SetFloat(BackgroundMusicVolumeKey, value);
    }

    public float SoundFxVolume
    {
        get => PlayerPrefs.GetFloat(SoundFxVolumeKey, 1.0f);
        set => PlayerPrefs.SetFloat(SoundFxVolumeKey, value);
    }

    public List<int> NormalHatIds
    {
        get => GetIntList(NormalHatIdsKey);
        set => SetIntList(NormalHatIdsKey, value);
    }

    private static void SetIntList(string key, List<int> value) // [1,2,3]
    {
        var commaSeparatedString = string.Join<int>(",", value); //  "1,2,3"
        PlayerPrefs.SetString(key, commaSeparatedString);
    }

    private static List<int> GetIntList(string key)
    {
        var commaSeparatedString = PlayerPrefs.GetString(key, ""); // "1,2,3"
        if (commaSeparatedString.Length == 0)
        {
            return new List<int>();
        }
        else
        {
            var arrayOfStrings = commaSeparatedString.Split(','); // ["1", "2", "3"]
            return arrayOfStrings.Select(int.Parse).ToList(); // [1, 2, 3]
        }
    }

    private void Awake()
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
    }
}
