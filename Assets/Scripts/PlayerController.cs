using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------
    [Header("Speed")]
    [Tooltip("Current maximum speed")]
    public float speed;

    [Tooltip("Speed change")]
    public float velocityChange = 0.5f;

    [Tooltip("Current velocity")]
    public Vector2 velocity;

    [Tooltip("Edges currently being touched")]
    private readonly HashSet<Edge> touchingEdges = new HashSet<Edge>();

    private enum Edge { North, South, East, West }

    // --------------------------------------------------------------
    [Header("Touch Controller")]

    [Tooltip("Radius of the controller in cm")]
    public float controllerRadiusCm = 2.0f;

    [Tooltip("Conversion of inches to centimeters")]
    private const float CmInInch = 2.54f;

    [Tooltip("Center of the controller (where touches began)")]
    private Vector2 centerOfController;

    public Vector3 FishPosition;
    
    // --------------------------------------------------------------
    [Header("body parts")] 
    public GameObject eye0;
    public GameObject eye1;

    // --------------------------------------------------------------
    [FormerlySerializedAs("Sprite")]
    [Header("Looks")]

    [Tooltip("Sprite")]
    public Sprite[] sprite;

    // --------------------------------------------------------------
    [Header("Cache")] 
    [Tooltip("Transform")]
    private Transform myTransform;

    [Tooltip("Renderer")]
    private Renderer myRenderer;

    private void Start()
    {
        myTransform = transform;
        myRenderer = GetComponent<Renderer>();
        GetComponent<SpriteRenderer>().sprite = sprite[Random.Range(0, sprite.Length - 1)];

    }

    private void Update()
    {
        myRenderer.enabled = GameManager.instance.state != GameManager.State.Menu;
              
        if (GameManager.instance.state == GameManager.State.Playing ||
            GameManager.instance.state == GameManager.State.Dead)
        {
          var axis = GetAxis();
          UpdateOrientation(axis);
          UpdateVelocity(axis);

           myTransform.Translate(velocity * Time.deltaTime, Space.World);
        }

        if (GameManager.instance.state == GameManager.State.Fish)
        {
            transform.position = FishPosition;
            transform.localScale = new Vector3(0.2287932f, 0.2287932f, 0.2287932f); 
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Edge")) 
        {
            touchingEdges.Add((Edge)Enum.Parse(typeof(Edge), other.gameObject.name));
        }
        if (other.gameObject.CompareTag("Plastic")) {
            print("ugh!");
        }

        if (other.gameObject.CompareTag("rareFish1"))
        {
            Hats.instance.Unlock(4);
        }

        if (other.gameObject.CompareTag("crab"))
        {
            if (Hats.instance.timesTouchedCrab == 200)
            {
                Hats.instance.Unlock(6);
            }
            else
            {
                Hats.instance.timesTouchedCrab += 1;
            }
        }

        if (other.gameObject.CompareTag("Waterplant"))
        {
             if (Hats.instance.timesTouchedCrab == 5000)
             {
                 Hats.instance.Unlock(4);
             }
             else
             {
                 Hats.instance.timesTouchedWaterplant += 1;
             }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Edge")) 
        {
            touchingEdges.Remove((Edge)Enum.Parse(typeof(Edge), other.gameObject.name));
        }
    }

    private void UpdateOrientation(Vector2 axis) {
        if (axis.x < 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (axis.x > 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    private void UpdateVelocity(Vector2 axis) {
        if (GameManager.instance.state != GameManager.State.Fish)
        {
            var targetVelocity = new Vector3(axis.x * speed, axis.y * speed, 0.0f);
            targetVelocity = ZeroVelocityAtEdge(targetVelocity);
            velocity = Vector3.Lerp(velocity, targetVelocity, velocityChange * Time.deltaTime);    
        }
        
    }

    private Vector3 ZeroVelocityAtEdge(Vector3 targetVelocity)
    {
        foreach (var edge in touchingEdges)
        {
            switch (edge)
            {
                case Edge.North when targetVelocity.y > 0.0f:
                case Edge.South when targetVelocity.y < 0.0f:
                    targetVelocity.y = 0.0f;
                    break;
                case Edge.East when targetVelocity.x > 0.0f:
                case Edge.West when targetVelocity.x < 0.0f:
                    targetVelocity.x = 0.0f;
                    break;
            }
        }

        return targetVelocity;
    }

    private Vector2 GetAxis()
    {
        if (GameManager.instance.state == GameManager.State.Playing)
        {
            return Vector2.ClampMagnitude(ReadController(), 1.0f);
        }
        if (GameManager.instance.state == GameManager.State.Dead)
        {
            return SimulateController();
        }
        return Vector2.zero;
    }

    private Vector2 SimulateController()
    {
        var x = Random.Range(0.4f, 0.7f);
        var y = Random.Range(0.1f, 0.2f);

        var goingWestAndNotAtTheEnd = velocity.x < 0.0f && !touchingEdges.Contains(Edge.West);
        var goingEastAndAtTheEnd = velocity.x > 0.0f && touchingEdges.Contains(Edge.East);
 
        if (goingWestAndNotAtTheEnd || goingEastAndAtTheEnd)
        {
            x *= -1;
        }

        var goingSouthAndNotAtTheEnd = velocity.y < 0.0f && !touchingEdges.Contains(Edge.South);
        var goingNorthAndAtTheEnd = velocity.y > 0.0f && transform.position.y > GameManager.instance.height * -0.2f;

        if (goingSouthAndNotAtTheEnd || goingNorthAndAtTheEnd)
        {
            y *= -1;
        }
        
        return new Vector2(x, y);
    }

    private Vector2 ReadController()
    {
        if (PersistencyManager.instance.Accelerometer && SystemInfo.supportsAccelerometer)
        {
            return ReadAccelerometerController();
        }
        else if (IsUsingTouchController())
        {
            return ReadTouchController();
        }
        else
        {
            return ReadKeyboardController();
        }
    }
    
    private bool IsUsingTouchController()
    {
        if (Input.GetMouseButtonDown(0))
        {
            centerOfController = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        return Input.GetMouseButton(0);
    }

    private static Vector2 ReadAccelerometerController()
    {
        var movementAccelerometer = Input.acceleration;

        return new Vector2(movementAccelerometer.x, movementAccelerometer.y);
    }
    
    private Vector2 ReadTouchController()
    {
        var controllerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var offsetInPixels = controllerPosition - centerOfController;
        var offsetInCm = offsetInPixels / Screen.dpi * CmInInch;

        return offsetInCm / controllerRadiusCm;
    }

    private static Vector2 ReadKeyboardController()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void Become(GameObject enemy)
    {
        touchingEdges.Clear();
        myTransform.position = enemy.transform.position;
        myTransform.rotation = enemy.transform.rotation;
        myTransform.localScale = enemy.transform.localScale;
        GetComponent<SpriteRenderer>().sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        velocity = new Vector2(enemy.GetComponent<FishMovement>().speed, 0);
        eye0.transform.localPosition = enemy.GetComponent<FishMovement>().eye0.transform.localPosition;
        eye0.transform.localScale = enemy.GetComponent<FishMovement>().eye0.transform.localScale;
        eye1.transform.localPosition = enemy.GetComponent<FishMovement>().eye1.transform.localPosition;
        eye1.transform.localScale = enemy.GetComponent<FishMovement>().eye1.transform.localScale;
    }
}
