﻿using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LayerGenerator[] layers;

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

    public void CreateLevel() {
        foreach(var layer in layers) {
            layer.Generate();
        }
    }
}
