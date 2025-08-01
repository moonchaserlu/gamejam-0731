using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float pushForce = 3f;  

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("UI Hints")]
    [Tooltip("????????????'?? E ??'")]
    public GameObject pushHintUI;

   
    private Rigidbody2D rb;
    private Interactable currentInteractable;
    private Pushable currentPushable;   
    private Pushable lockedPushable;    
    private bool isPushing = false;     
    private bool controlsEnabled = true;

    
    private float rebindCooldownUntil = 0f;
    private const float REBIND_COOLDOWN = 0.05f; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (pushHintUI != null) pushHintUI.SetActive(false);
    }

    void Update()
    {
        if (!controlsEnabled) return;

        
        HandleMovement();

        
        HandlePushOrInteractWithE();

        
        if (pushHintUI != null)
            pushHintUI.SetActive(currentPushable != null && !isPushing);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float moveY = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // 计算实际移动速度（考虑推动时的减速）
        float speed = (isPushing ? moveSpeed * 0.7f : moveSpeed);
        rb.velocity = movement * speed;

        // 推动物体时，使用相同的移动向量但保持原始速度
        if (isPushing && lockedPushable != null)
        {
            // 这里使用movement.normalized确保方向准确
            lockedPushable.Push(movement.normalized * moveSpeed);
        }

        if (animator) animator.SetBool("isWalking", movement.sqrMagnitude > 0.0001f);
        if (spriteRenderer && moveX != 0) spriteRenderer.flipX = (moveX < 0);
    }

    private void HandlePushOrInteractWithE()
    {
        bool eDown = Input.GetKeyDown(KeyCode.E);
        bool eUp = Input.GetKeyUp(KeyCode.E);

        
        if (eDown)
        {
            
            if (!isPushing && Time.time >= rebindCooldownUntil && currentPushable != null)
            {
                StartPushLocked(currentPushable);
                
                return;
            }
        }

        
        if (eUp && isPushing)
        {
            StopPushLocked();
            return;
        }

        
        if (eDown && !isPushing && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void StartPushLocked(Pushable target)
    {
        lockedPushable = target;
        isPushing = true;
        lockedPushable.StartPushing(this);
        
    }

    private void StopPushLocked()
    {
        isPushing = false;
        if (lockedPushable != null)
        {
            lockedPushable.StopPushing();
            lockedPushable = null;
        }
        rebindCooldownUntil = Time.time + REBIND_COOLDOWN;
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.GetComponentInParent<Interactable>();
        }
        else if (other.CompareTag("Pushable"))
        {
            
            if (!isPushing && Time.time >= rebindCooldownUntil)
            {
                currentPushable = other.GetComponentInParent<Pushable>();
            }
            
        }
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

            
            if (isPushing && lockedPushable == p)
            {
                StopPushLocked();
            }

            
            if (!isPushing && currentPushable == p)
            {
                currentPushable = null;
            }

            
        }
    }

    
    public void EnableControls() => controlsEnabled = true;

    public void DisableControls()
    {
        controlsEnabled = false;
        rb.velocity = Vector2.zero;
        if (animator) animator.SetBool("isWalking", false);

        
        if (isPushing) StopPushLocked();
    }

    public void ResetPlayer()
    {
        EnableControls();
    }
}
