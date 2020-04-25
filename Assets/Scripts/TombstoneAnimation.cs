using UnityEngine;

public class TombstoneAnimation : MonoBehaviour
{
    public Transform alive;

    public Transform dead;
    
    public GameObject tombstone;

    public float speed = 1.0f;

    private void Start()
    {
        tombstone.transform.position = alive.position;
    }

    private void Update()
    {
        var died = GameManager.instance.state == GameManager.State.Dead;
        var change = speed * Time.deltaTime;

        tombstone.transform.position = Vector3.Lerp(
            tombstone.transform.position,
            died ? dead.position : alive.position,
            change);
    }
}
