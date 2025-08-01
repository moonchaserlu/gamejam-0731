using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider2D doorCollider;

    public bool IsPlayerPassing { get; private set; }
    protected bool isLocked = true;

    public virtual void Lock()
    {
        isLocked = true;
        if (animator) animator.SetBool("isLocked", true);
    }

    public virtual void Unlock()
    {
        isLocked = false;
        if (animator) animator.SetBool("isLocked", false);
    }

    public virtual void OnInteract()
    {
        if (isLocked) return;

        if (animator) animator.SetTrigger("Open");
        if (doorCollider) doorCollider.enabled = false;
        // ??????????????
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLocked && other.CompareTag("Player"))
        {
            IsPlayerPassing = true;
        }
    }

    /// <summary>
    /// ????????????????????????
    /// </summary>
    public void ResetPassingFlag()
    {
        IsPlayerPassing = false;
    }
}
