using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushSpeedMultiplier = 1.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pushStartSound;
    public AudioClip pushMoveSound;

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

        if (audioSource && pushStartSound)
            audioSource.PlayOneShot(pushStartSound);
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

        rb.velocity = force * pushSpeedMultiplier;
    }
}