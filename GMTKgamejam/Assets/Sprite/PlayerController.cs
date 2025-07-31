using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float pushForce = 3f;
    public Transform playerSpawnPoint;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Interactable currentInteractable;
    private Pushable currentPushable;
    private bool controlsEnabled = true;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (!controlsEnabled) { return; }

        HandleMovement();

        // 修正后的交互和推动代码
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandlePushing();
        }
    }


    private void HandleMovement()
    {
        float moveX = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float moveY = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb.velocity = movement * (currentPushable ? moveSpeed * 0.7f : moveSpeed);

        if (currentPushable != null)
            currentPushable.Push(movement * pushForce);

        animator.SetBool("isWalking", movement.magnitude > 0);
        if (moveX != 0) spriteRenderer.flipX = moveX < 0;
    }

    private void HandlePushing()
    {
        if (currentPushable == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
            currentPushable.StartPushing(this);
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            currentPushable.StopPushing();
            currentPushable = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
            currentInteractable = other.GetComponent<Interactable>();
        else if (other.CompareTag("Pushable"))
            currentPushable = other.GetComponent<Pushable>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && currentInteractable?.gameObject == other.gameObject)
            currentInteractable = null;
        else if (other.CompareTag("Pushable") && currentPushable?.gameObject == other.gameObject)
        {
            currentPushable.StopPushing();
            currentPushable = null;
        }
    }

    public void EnableControls() => controlsEnabled = true;
    public void DisableControls()
    {
        controlsEnabled = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    public void ResetPlayer()
    {
        transform.position = playerSpawnPoint.position;
        EnableControls();
    }
}