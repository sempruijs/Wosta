using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Time between spawns (seconds)")]
    [Tooltip("Minimum (First Level)")]
    public float minTimeFirstLevel = 5.0f;

    [Tooltip("Maximum (First Level)")]
    public float maxTimeFirstLevel = 15.0f;

    [Tooltip("Minimum (Last Level)")]
    public float minTimeLastLevel = 3.0f;

    [Tooltip("Maximum (Last Level)")]
    public float maxTimeLastLevel = 10.0f;

    // --------------------------------------------------------------
    [Header("Prefabs")]
    [Tooltip("Prefabs")]
    public GameObject[] prefabs;
    public GameObject rareFish;

    // --------------------------------------------------------------
    [Header("Between")]
    [Tooltip("Other point")] 
    public Transform other;

    // --------------------------------------------------------------
    [Header("Cache")]
    [Tooltip("Origin position")] 
    public Vector3 originPosition;
    
    [Tooltip("Vector from this position to other point")] 
    public Vector3 deltaPosition;
    
    
    private int changesOfSpawningRareFish;

    private void Start()
    {
        originPosition = transform.position;
        deltaPosition = other.position - originPosition;

        
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
 
        
        while (true)
        {
            if (GameManager.instance.state == GameManager.State.Playing || GameManager.instance.state == GameManager.State.Menu)
            {

                var prefab = prefabs[Random.Range(0, prefabs.Length)];
                
                
                if (changesOfSpawningRareFish == 4) {
                        if (!Hats.instance.ownedHatIDsNormal.Contains(4))
                        {
                            prefab = rareFish;
                        }
                } 
                
            
                var targetPosition = originPosition + Random.value * deltaPosition + GameManager.instance.Zposition;
            
                var instance = Instantiate(prefab, targetPosition, transform.rotation);
                if (GameManager.instance.Zposition.z >= -6f)
                {
                    GameManager.instance.Zposition.z -= 0.03f;    
                }
                else
                {
                    GameManager.instance.Zposition.z = 0f;
                }
                
                instance.transform.parent = transform;
            }

            yield return new WaitForSeconds(NextWaitTime());
            changesOfSpawningRareFish = Random.Range(0, 10000);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private float NextWaitTime()
    {
        var minTime = minTimeFirstLevel + (minTimeLastLevel - minTimeFirstLevel) * GameManager.instance.Progress();
        var maxTime = maxTimeFirstLevel + (maxTimeLastLevel - maxTimeFirstLevel) * GameManager.instance.Progress();
        
        return Random.Range(minTime, maxTime);
    }
}
