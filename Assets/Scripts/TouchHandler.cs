using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.EatEnemy(gameObject, other.gameObject);
        }
        else if (other.gameObject.CompareTag("Plastic"))
        {
            GameManager.instance.EatPlastic(gameObject, other.gameObject);
        }
    }
}
