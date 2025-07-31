using UnityEngine;

public abstract class Door : MonoBehaviour
{
    public Animator animator;
    public Collider2D doorCollider;
    public bool isLocked = true;
    public bool IsPlayerPassing { get; protected set; }

    public virtual void Unlock()
    {
        isLocked = false;
        animator.SetBool("isLocked", false);
    }

    public virtual void Lock()
    {
        isLocked = true;
        IsPlayerPassing = false;
        animator.SetBool("isLocked", true);
    }

    public virtual void OnInteract()
    {
        if (!isLocked)
        {
            animator.SetTrigger("Open");
            doorCollider.enabled = false;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isLocked)
            IsPlayerPassing = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsPlayerPassing = false;
    }
}