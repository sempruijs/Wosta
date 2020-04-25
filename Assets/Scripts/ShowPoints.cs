using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  
public class ShowPoints: MonoBehaviour
{
    public float speed;

    private TMP_Text tmpText;
  
    private void Start()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void Update() {
         transform.Translate(Time.deltaTime * speed * Vector3.up, Space.World);
        tmpText.text = GameManager.instance.scoreForFish.ToString("F0");
        Destroy(gameObject, 0.4f);

    }
}
