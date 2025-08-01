using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushSpeedMultiplier = 1.0f; // 改为乘数而非固定速度

    private Rigidbody2D rb;
    private bool isPushing = false;
    private PlayerController pusher;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartPushing(PlayerController who)
    {
        isPushing = true;
        pusher = who;
    }

    public void StopPushing()
    {
        isPushing = false;
        pusher = null;

        if (rb != null) rb.velocity = Vector2.zero;
    }

    public void Push(Vector2 force)
    {
        if (!isPushing || rb == null) return;

        // 使用玩家的移动速度乘以乘数
        rb.velocity = force * pushSpeedMultiplier;
    }
}