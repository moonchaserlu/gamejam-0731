using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float pushForce = 3f;             // ????????? Pushable.pushSpeed ???????

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("UI Hints")]
    [Tooltip("?????????????????? Space ???")]
    public GameObject pushHintUI;            // ? ? Canvas ?????????

    private Rigidbody2D rb;
    private Interactable currentInteractable;
    private Pushable currentPushable;
    private bool controlsEnabled = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (pushHintUI != null) pushHintUI.SetActive(false);
    }

    void Update()
    {
        if (!controlsEnabled) { return; }

        HandleMovement();

        // ????E??????
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }

        // ???Space????/?????????????????????
        HandlePushing();

        // ?? UI?????“?? Space”
        if (pushHintUI != null)
            pushHintUI.SetActive(currentPushable != null);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float moveY = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // ?????????????????? 0.7 ????
        float speed = (currentPushable ? moveSpeed * 0.7f : moveSpeed);
        rb.velocity = movement * speed;

        // ???“???”???????????
        if (currentPushable != null)
            currentPushable.Push(movement * pushForce);

        // ??/??
        if (animator) animator.SetBool("isWalking", movement.sqrMagnitude > 0.0001f);
        if (spriteRenderer && moveX != 0) spriteRenderer.flipX = (moveX < 0);
    }

    private void HandlePushing()
    {
        if (currentPushable == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
            currentPushable.StartPushing(this);

        if (Input.GetKeyUp(KeyCode.Space))
            currentPushable.StopPushing(); // ?????????????????
    }

    // —— ????????“?????? + ???? Pushable/Interactable” ——
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
            currentInteractable = other.GetComponentInParent<Interactable>();

        else if (other.CompareTag("Pushable"))
            currentPushable = other.GetComponentInParent<Pushable>();     // ? ??????????
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            var inter = other.GetComponentInParent<Interactable>();
            if (currentInteractable == inter) currentInteractable = null;
        }
        else if (other.CompareTag("Pushable"))
        {
            var p = other.GetComponentInParent<Pushable>();
            if (currentPushable != null) currentPushable.StopPushing();
            if (currentPushable == p) currentPushable = null;
        }
    }

    public void EnableControls() => controlsEnabled = true;

    public void DisableControls()
    {
        controlsEnabled = false;
        rb.velocity = Vector2.zero;
        if (animator) animator.SetBool("isWalking", false);
        if (currentPushable != null) currentPushable.StopPushing();
    }

    public void ResetPlayer()
    {
        EnableControls();
    }
}
