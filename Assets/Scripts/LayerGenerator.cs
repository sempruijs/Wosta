using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LayerGenerator : MonoBehaviour
{
    public Camera targetCamera;

    // --------------------------------------------------------------
    [Header("Assets")] 
    [Tooltip("Assets")]
    public GameObject[] assets;

    [Tooltip("Minimum number of assets")]
    public int assetCountMin = 1;

    [Tooltip("Maximum number of assets")]
    public int assetCountMax = 5;

    [Header("Size")] 
    [Tooltip("Space between assets")]
    public float margin = 0.5f;

    [Tooltip("Space between assets")]
    public float baseSize = 1.0f;

    [Tooltip("Size variation (in percentage)")]
    public float resize = 0.2f;

    [Header("Animation")]
    [Tooltip("Animations speed")]
    public float animationSpeed = 1.0f;

    [Tooltip("Animation speed variation (in percentage)")]
    public float animationSpeedVariation = 0.2f;

    [Header("Cache")] 
    [Tooltip("Transform")]
    public Transform myTransform;
    
    [Tooltip("Position")]
    public Vector3 myPosition;

    [Tooltip("Scaling factor of layer (based on position.z)")]
    public float factor;

    private void Start()
    {
        myTransform = transform;
        myPosition = myTransform.position;
        
        factor = myPosition.z > 0.0f ? 1 / myPosition.z : myPosition.z * -1;
    }

    private void Clear() 
    {
         foreach (Transform child in transform) 
         {
            Destroy(child.gameObject);
         }
         
         myTransform.position = new Vector3( 0.0f, 0.0f, myTransform.position.z);
    }

    public void Generate() 
    {
        Clear();

        var gameHeight = GameManager.instance.height * factor;
        var gameWidth = GameManager.instance.width * factor;

        var count = Random.Range(assetCountMin, assetCountMax);

        // X-ranges taken by other assets
        var taken = new List<RangeAttribute>();

        foreach (var _ in Enumerable.Range(0, count)) {
            // Select an asset
            var prefab = assets[Random.Range(0, assets.Length)];

            // Get the size of the asset
            var size = prefab.GetComponentInChildren<Renderer> ().bounds.size;
            var prefabWidth = size.x;

            // Indicator if position is found
            bool free;
            
            // Selected position
            float x;
            
            // Limit the number of attempts (to prevent endless loop)
            var tryCount = 0;
            do {
                tryCount++;
                free = true;

                // Select random position of asset
                x = Random.Range(gameWidth / 2 * -1, gameWidth / 2);

                // Check if this position if taken by another asset
                foreach (var candidate in taken) {
                    free &= (x + prefabWidth / 2 < candidate.min || x - prefabWidth / 2 > candidate.max);
                }
            } while (!free && tryCount < 10);

            if (free) {
                // Select random size
                var resizeProduct = Random.Range(baseSize - resize, baseSize + resize);

                // Calculate y-position based on prefab height
                var y = gameHeight / 2 * -1;

                // Create game object
                var instance = Instantiate(
                    prefab, 
                    new Vector3(x, y, 0.0f),
                    Quaternion.identity);

                // Set the parent
                instance.transform.parent = transform;

                // Set z-order to layer z
                instance.transform.localPosition = 
                    new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y, myTransform.position.z);

                // Scale the game object
                instance.transform.localScale *= resizeProduct;
                var resizedWidth = prefabWidth * resizeProduct;

                var position = instance.transform.position;

                var animator = instance.GetComponent<Animator>();
                if (animator != null)
                {
                    var state = animator.GetCurrentAnimatorStateInfo(0);
                    animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
                    animator.speed = Random.Range(animationSpeed - animationSpeedVariation, animationSpeed + animationSpeedVariation); 
                }


                taken.Add(new RangeAttribute(
                    position.x - resizedWidth / 2 * resizeProduct * (1.0f + margin),
                    position.x + resizedWidth / 2 * resizeProduct * (1.0f + margin)));
            }
        }
    }
    
    public void Update() {
        var height = GameManager.instance.height;
        var width = GameManager.instance.width;

        var cameraSize = targetCamera.orthographicSize;
        
        var currentCameraY = targetCamera.transform.position.y;
        var maxCameraY = height / 2 - cameraSize;

        var targetLayerY = 0.0f;
        if (Math.Abs(maxCameraY) > 0.001f) {
            var cameraPercentageY = currentCameraY / maxCameraY;
            var maxLayerY = (height * factor - height) / 2;
            targetLayerY = maxLayerY * cameraPercentageY * -1;
        }
        
        var currentCameraX = targetCamera.transform.position.x;
        var maxCameraX = width / 2 - cameraSize * targetCamera.aspect;

        var targetLayerX = 0.0f;
        if (Math.Abs(maxCameraX) > 0.001f) 
        {
            var cameraPercentageX = currentCameraX / maxCameraX;
            var maxLayerX = (width * factor - width) / 2;
            targetLayerX = maxLayerX * cameraPercentageX * 2 * -1;
        }
        
        myTransform.position = new Vector3(targetLayerX, targetLayerY, myTransform.position.z);
    }
}
