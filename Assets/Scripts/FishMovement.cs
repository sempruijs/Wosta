using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Movement")]
    [Tooltip("Minimum speed")]
    public float minimumSpeed = 1.0f;
    
    [Tooltip("Maximum speed (first level)")]
    public float maximumSpeedFirstLevel = 1.5f;

    [Tooltip("Maximum speed (last level)")]
    public float maximumSpeedLastLevel = 3.0f;

    [Tooltip("Acceleration when bot is fleeing")]
    public float fleeingAcceleration = 5.0f;

    [Tooltip("Speed")]
    public float speed;

    // --------------------------------------------------------------
    [Header("Size")]
    [Tooltip("Maximum size")]
    public float minimumSize = 0.03f;

    [Tooltip("Maximum size")]
    public float maximumSize = 0.4f;

    // --------------------------------------------------------------
    [Header("Fish bodys")]
    public Sprite[] fish;
    
    // --------------------------------------------------------------
    [Header("body parts")] 
    public GameObject eye0;
    public GameObject eye1;

    // --------------------------------------------------------------
    [Header("Boundary check")]
    [Tooltip("Starting position to calculate if the plastic reached the other side of the screen")]
    private Vector3 startingPosition;

    // --------------------------------------------------------------
    [Header("Cache")]
    [Tooltip("Transform")]
    private Transform myTransform;

    // --------------------------------------------------------------
    [Header("Static")]
    [Tooltip("Games states with enabled fish")]
    private readonly HashSet<GameManager.State> fishAvailableStates = new HashSet<GameManager.State>
    {
        GameManager.State.Menu,
        GameManager.State.Pause,
        GameManager.State.Playing,
        GameManager.State.NextLevel,
        GameManager.State.Dead,
        GameManager.State.Settings
    };

    [Tooltip("Games states with fleeing fish")]
    private readonly HashSet<GameManager.State> fishFleeingStates = new HashSet<GameManager.State>
    {
        GameManager.State.NextLevel,
        GameManager.State.Dead
    };

    // Start is called before the first frame update
    private void Start()
    {
        myTransform = transform;

        startingPosition = myTransform.position;

        // Select speed
        var maximumSpeed = maximumSpeedFirstLevel +
                           (maximumSpeedLastLevel - maximumSpeedFirstLevel) * GameManager.instance.Progress();
        speed = Random.Range(minimumSpeed, maximumSpeed);
        if (startingPosition.x > 0) {
            speed *= -1;
            myTransform.rotation = Quaternion.Euler(0f, 180f, 0f); 
        } 

        // Select fish type
        var index = Random.Range (0, fish.Length);
        GetComponent<SpriteRenderer>().sprite = fish[index];

        // Select fish size
        var randomScale = Random.Range(minimumSize, maximumSize);
        myTransform.localScale = new Vector3(randomScale, randomScale, randomScale);
        
        // Select eye size
        var randomEyeScale = Random.Range(0.06f, 0.09f);
        eye0.transform.localScale = new Vector3(randomEyeScale, randomEyeScale, randomEyeScale);
        eye1.transform.localScale = new Vector3(randomEyeScale, randomEyeScale, randomEyeScale);
        
        // Select eye position
        //(3.747 && 0.384f is the standard position)
        var randomEyePositionX = Random.Range(3.5f, 3.35f);
        var randomEyePositiony = Random.Range(0.15f, 0.45f);
        eye0.transform.localPosition = new Vector3(randomEyePositionX, randomEyePositiony, -0.001f);
        eye1.transform.localPosition = new Vector3(randomEyePositionX, randomEyePositiony, 0.001f);
    }

    private void Update()
    {
        if (FishAvailable())
        {
            if (OutOfScreen())
            {
                Destroy(gameObject);
            }
            else if (!GameManager.instance.IsPaused()) 
            {
                if (FishFleeing()) {
                    speed *= 1 + fleeingAcceleration * Time.deltaTime;
                }
           
                myTransform.Translate(Time.deltaTime * speed * Vector3.right, Space.World);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool OutOfScreen()
    {
        var position = myTransform.position;
        
        return
            (startingPosition.x < 0 && startingPosition.x + position.x > 0) ||
            (startingPosition.x > 0 && startingPosition.x + position.x < 0);
    }

    private bool FishAvailable()
    {
        return fishAvailableStates.Contains(GameManager.instance.state);
    }

    private bool FishFleeing()
    {
        return fishFleeingStates.Contains(GameManager.instance.state);
    }
}
