using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameObject interactionPrompt;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public abstract void Interact();
}