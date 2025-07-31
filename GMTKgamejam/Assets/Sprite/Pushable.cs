using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float pushSpeed = 2f;
    private Rigidbody2D rb;
    private bool isBeingPushed = false;
    private PlayerController pusher;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void StartPushing(PlayerController player)
    {
        isBeingPushed = true;
        pusher = player;
    }

    public void StopPushing()
    {
        isBeingPushed = false;
        pusher = null;
        rb.velocity = Vector2.zero;
    }

    public void Push(Vector2 force)
    {
        if (isBeingPushed)
            rb.velocity = force * pushSpeed;
    }
}