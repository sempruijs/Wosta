using System;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Singleton")]
    [Tooltip("Singleton instance")]
    public static CameraManager instance;

    // --------------------------------------------------------------
    [Header("Camera")]
    [Tooltip("Target camera to move")]
    public Camera targetCamera;

    [Tooltip("Maximum percentage of the height / width that is visible")]
    [Range(0.0f, 1.0f)]
    public float viewportRatio = 0.8f;

    // --------------------------------------------------------------
    [Header("Movement")]
    [Tooltip("Speed the camera follows the player")]
    public float cameraSpeed = 3.0f;

    [Tooltip("Player that is tracked")]
    public GameObject player;

    // --------------------------------------------------------------
    [Header("Transition")]

    [Tooltip("Speed to transition")]
    public float transitionSpeed = 10.0f;

    [Tooltip("Speed to reload")]
    public float reloadSize = 1.0f;

    [Tooltip("Currently in transition indicator")]
    private bool transition;
    
    [Tooltip("Position of the camera when the game started")]
    private Vector3 originalCameraPosition;
    
    // --------------------------------------------------------------
    [Header("Caching")]
    [Tooltip("Hash of camera aspect and viewport ratio to prevent camera-size changes")]
    private string lastHash;
    
    // --------------------------------------------------------------
    [Header("Debug")]
    [Tooltip("Enable debug logging")]
    public bool debug;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);    
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        lastHash = "";
        transition = false;
        originalCameraPosition = targetCamera.transform.position;
    }

    private void Update()
    {
        // Set camera size
        if (lastHash != CameraHash()) {
            lastHash = CameraHash();

            targetCamera.orthographicSize = TargetCameraSize();
        }

        // Set camera position
        if (!transition) {
            if (debug) Debug.Log("Moving Camera");

            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position,
                TargetCameraPosition(),
                cameraSpeed * Time.deltaTime);
        }
    }

    private string CameraHash () {
        return "aspect: " + targetCamera.aspect + "viewportRatio: " + viewportRatio;
    }

    private float TargetCameraSize () {
        var height = GameManager.instance.height;
        var width = GameManager.instance.width;

        var maximumSize = height / 2;
        var optimalAspect = width / height;

        if (targetCamera.aspect > optimalAspect) {
            // Screen is too wide; reduce the heigth
            return maximumSize * optimalAspect / targetCamera.aspect * viewportRatio;
        } else {
            // Screen is too narrow; set size to maximum
            return maximumSize * viewportRatio;
        }
    }

    private Vector3 TargetCameraPosition () {
        var height = GameManager.instance.height;
        var width = GameManager.instance.width;
        var position = player.transform.position;

        // Calculate X Movement
        var marginX = width / 2 - targetCamera.orthographicSize * targetCamera.aspect;
        var offsetX = position.x - originalCameraPosition.x;
        var offsetPercentageX = offsetX * 2 / width;
        var cameraMovementX = marginX * offsetPercentageX;
        var cameraX = originalCameraPosition.x + cameraMovementX;

        // Calculate Y Movement
        var marginY = height / 2 - targetCamera.orthographicSize;
        var offsetY = position.y - originalCameraPosition.y;
        var offsetPercentageY = offsetY * 2 / height;
        var cameraMovementY = marginY * offsetPercentageY;
        var cameraY = originalCameraPosition.y + cameraMovementY;

        return new Vector3(cameraX, cameraY, originalCameraPosition.z);
    }

    public void Transition (Action prepare, Action complete) {
        StartCoroutine(DoTransition(prepare, complete));
    }

    private IEnumerator DoTransition (Action prepare, Action complete) {
        transition = true;

        while (Mathf.Abs(targetCamera.orthographicSize - reloadSize) > 0.01) {
            // zoom in
            targetCamera.orthographicSize = Mathf.Lerp(targetCamera.orthographicSize, reloadSize, transitionSpeed * Time.deltaTime);
            var currentCamera = targetCamera.transform.position;
            var currentPlayer = player.transform.position;

            targetCamera.transform.position =  new Vector3(
                Mathf.Lerp(currentCamera.x, currentPlayer.x, transitionSpeed * Time.deltaTime),
                Mathf.Lerp(currentCamera.y, currentPlayer.y, transitionSpeed * Time.deltaTime),
                currentCamera.z);

            yield return null;
        }

        prepare();

        targetCamera.transform.position = TargetCameraPosition();

        yield return null;

        while (Mathf.Abs(targetCamera.orthographicSize - TargetCameraSize()) > 0.01) {
            targetCamera.orthographicSize = Mathf.Lerp(targetCamera.orthographicSize, TargetCameraSize(), transitionSpeed * Time.deltaTime);
            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position,
                TargetCameraPosition(),
                transitionSpeed * Time.deltaTime);

            yield return null;
        }

        complete();

        transition = false;
    }
}
