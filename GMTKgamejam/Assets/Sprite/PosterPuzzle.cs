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

    [Header("Posters")]
    public Poster[] posters;

    [Header("Solution (use color index starting at 0)")]
    public int[] solution = { 1, 0, 1 };

    [Header("FX")]
    public float flashDuration = 0.3f;

    [Header("Interact")]
    public float interactRadius = 2.5f;   

    public CarriageLoopCoroutine loopSystem;

    private bool isActive = false;

    public override void Interact()
    {
        if (!isActive) return;                      

        
        var hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        Transform player = GameManager.Instance.playerController.transform;

        int nearestIndex = -1;
        float nearestDistSqr = float.MaxValue;

        foreach (var col in hits)
        {
            if (!col.CompareTag("Poster")) continue; 

            
            int idx = System.Array.FindIndex(posters, p => p.renderer.gameObject == col.gameObject); 
            if (idx == -1) continue;

            float d2 = (col.transform.position - player.position).sqrMagnitude;
            if (d2 < nearestDistSqr)
            {
                nearestDistSqr = d2;
                nearestIndex = idx;
            }
        }

        if (nearestIndex != -1)
            StartCoroutine(ChangePosterColor(nearestIndex));
    }

    private IEnumerator ChangePosterColor(int index)
    {
        Poster p = posters[index];

        
        p.renderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        
        p.currentState = (p.currentState + 1) % p.colors.Length;
        p.renderer.color = p.colors[p.currentState];

        
        ReportSolvedState();
    }

    private void ReportSolvedState()
    {
        bool solved = true;
        for (int i = 0; i < posters.Length; i++)
        {
            if (posters[i].currentState != solution[i]) { solved = false; break; }
        }
        
        loopSystem.SetPuzzleSolved(0, solved);
    }

    public void Activate()
    {
        isActive = true;

        
        foreach (var poster in posters)
        {
            poster.currentState = 0;
            if (poster.colors != null && poster.colors.Length > 0)
            {
                var c0 = poster.colors[0]; c0.a = 1f; poster.colors[0] = c0; 
                poster.renderer.color = poster.colors[0];
            }
        }

        
        ReportSolvedState();
    }

    public void ResetPuzzle()
    {
        isActive = false;
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
