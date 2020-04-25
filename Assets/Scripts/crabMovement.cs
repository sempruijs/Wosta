using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crabMovement : MonoBehaviour
{
    public float left = -10.0f;
    public float right = 6.45f;

    void Start() {
        transform.position = -new Vector3(6.45f, -3.5f, 0.2f); 
    }

    void Update()
    {
        transform.position = new Vector3(Mathf.Lerp (left, right, Mathf.PingPong(Time.time * 0.05f, 2)), -3.5f, 0.2f); 
    }
}
