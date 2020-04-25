using System.Collections.Generic;
using UnityEngine;

public class PlasticMovement : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Rotation")]
    [Tooltip("Minimum")]
    public float minimumRotation = 30.0f;

    [Tooltip("Minimum")] public float maximumRotation = 150.0f;

    // --------------------------------------------------------------
    [Header("Movement")]
    [Tooltip("Speed")]
    public float speed;

    // --------------------------------------------------------------
    [Header("Static")]
    [Tooltip("Games states with enabled pause functionality")]
    private HashSet<GameManager.State> plasticAvailableStates = new HashSet<GameManager.State>
    {
        GameManager.State.Pause,
        GameManager.State.Playing,
        GameManager.State.Settings
    };

    // --------------------------------------------------------------
    [Header("Boundary check")]
    [Tooltip("Starting position to calculate if the plastic reached the other side of the screen")]
    private Vector3 startingPosition;

    // --------------------------------------------------------------
    [Header("Caching")]
    [Tooltip("Transform")]
    private Transform myTransform;

    private GameObject crab;

    private void Start()
    {
        myTransform = transform;

        startingPosition = myTransform.position;

        var rotation = Random.Range(minimumRotation, maximumRotation);
        myTransform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private void Update()
    {
        if (PlasticAvailable())
        {
            if (crab != null) {
                myTransform.position = new Vector3(crab.transform.position.x, crab.transform.position.y + 0.35f, crab.transform.position.z); 
                Destroy(gameObject.GetComponent<EdgeCollider2D>());
                if (crab.transform.position.x >= 6.45f) {
                    Destroy(gameObject);
                }
            }
            else if (myTransform.position.y <= -3.5f) {
                myTransform.position = new Vector3(myTransform.position.x, -3.5f, myTransform.position.z);
            }
            else if (!GameManager.instance.IsPaused())
            {
                myTransform.Translate(Time.deltaTime * speed * Vector3.down, Space.World);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool OutOfScreen()
    {
        return startingPosition.y + transform.position.y < 0;
    }

    private bool PlasticAvailable()
    {
        return plasticAvailableStates.Contains(GameManager.instance.state);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (crab == null && collider.gameObject.tag == "crab")
        {
            crab = collider.gameObject;
            myTransform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }
}