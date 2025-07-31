using System.Collections;
using UnityEngine;

public class PosterPuzzle : Interactable
{
    [System.Serializable]
    public class Poster
    {
        public SpriteRenderer renderer;
        public Color[] colors;
        [HideInInspector] public int currentState = 0;
    }

    public Poster[] posters;
    public int[] solution = { 1, 2, 1 };
    public float flashDuration = 0.3f;
    public CarriageLoopCoroutine loopSystem;

    private bool isActive = false;

    public override void Interact()
    {
        if (!isActive) return;

        foreach (var col in Physics2D.OverlapCircleAll(transform.position, 1.5f))
        {
            if (!col.CompareTag("Poster")) continue;

            int index = System.Array.FindIndex(posters, p => p.renderer.gameObject == col.gameObject);
            if (index != -1)
            {
                StartCoroutine(ChangePosterColor(index));
                break;
            }
        }
    }

    private IEnumerator ChangePosterColor(int index)
    {
        Poster poster = posters[index];
        poster.renderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        poster.currentState = (poster.currentState + 1) % poster.colors.Length;
        poster.renderer.color = poster.colors[poster.currentState];
        CheckSolution();
    }

    private void CheckSolution()
    {
        for (int i = 0; i < posters.Length; i++)
        {
            if (posters[i].currentState != solution[i]) return;
        }
        isActive = false;
        loopSystem.CompletePuzzle(0);
    }

    public void Activate()
    {
        isActive = true;
        foreach (var poster in posters)
        {
            poster.currentState = 0;
            poster.renderer.color = poster.colors[0];
        }
    }

    public void ResetPuzzle() => isActive = false;
}