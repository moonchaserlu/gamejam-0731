using System.Collections;
using UnityEngine;

/// <summary>
/// ???? Sprite ???????????????
/// ????????? Puzzle ?????
/// ?????????? SpriteRenderer ? Tag = "Poster"?
/// </summary>
public class PosterPuzzle : Interactable
{
    #region ???? -----------------------------------------------------------------

    [System.Serializable]
    public class Poster
    {
        public SpriteRenderer renderer;   // ??????
        public Sprite[] sprites;     // ??? Sprite ??
        [HideInInspector] public int currentState = 0; // ?????? 0 ???
    }

    #endregion

    #region Inspector ?? -----------------------------------------------------------

    [Header("Posters")]
    public Poster[] posters;

    [Header("Solution (use sprite index starting at 0)")]
    public int[] solution = { 1, 0, 1 };

    [Header("FX")]
    public float flashDuration = 0.3f;        // ?????????

    [Header("Interact")]
    public float interactRadius = 2.5f;       // ???????

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip posterChangeSound;

    public CarriageLoopCoroutine loopSystem;  // ??????

    #endregion

    #region ????? ---------------------------------------------------------------

    private bool isActive = false;

    #endregion

    #region ????? ---------------------------------------------------------------

    public override void Interact()
    {
        if (!isActive) return;

        // 1. ???????? Poster
        var hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        Transform player = GameManager.Instance.playerController.transform;

        int nearestIndex = -1;
        float nearestDistSq = float.MaxValue;

        foreach (var col in hits)
        {
            if (!col.CompareTag("Poster")) continue;

            int idx = System.Array.FindIndex(posters, p => p.renderer.gameObject == col.gameObject);
            if (idx == -1) continue;

            float d2 = (col.transform.position - player.position).sqrMagnitude;
            if (d2 < nearestDistSq)
            {
                nearestDistSq = d2;
                nearestIndex = idx;
            }
        }

        // 2. ???? Poster ? Sprite
        if (nearestIndex != -1)
            StartCoroutine(ChangePosterSprite(nearestIndex));
    }

    #endregion

    #region ??????? Sprite -----------------------------------------------------

    private IEnumerator ChangePosterSprite(int index)
    {
        Poster p = posters[index];

        if (p.sprites == null || p.sprites.Length == 0)
            yield break; // ????? Sprite?????

        // ?? SFX
        if (audioSource && posterChangeSound)
            audioSource.PlayOneShot(posterChangeSound);

        // ? ????
        Color originalColor = p.renderer.color;
        p.renderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        p.renderer.color = originalColor;

        // ? ?????? Sprite
        p.currentState = (p.currentState + 1) % p.sprites.Length;
        p.renderer.sprite = p.sprites[p.currentState];

        // ? ???????
        ReportSolvedState();
    }

    #endregion

    #region ???? ------------------------------------------------------------------

    private void ReportSolvedState()
    {
        bool solved = true;
        for (int i = 0; i < posters.Length; i++)
        {
            if (posters[i].currentState != solution[i]) { solved = false; break; }
        }

        loopSystem.SetPuzzleSolved(0, solved);
    }

    #endregion

    #region ???? / ???? -------------------------------------------------------

    /// <summary>?????????????????</summary>
    public void Activate()
    {
        isActive = true;

        // ???? Poster ?????
        foreach (var poster in posters)
        {
            poster.currentState = 0;

            if (poster.sprites != null && poster.sprites.Length > 0)
                poster.renderer.sprite = poster.sprites[0];
        }

        ReportSolvedState();
    }

    /// <summary>?????????????</summary>
    public void ResetPuzzle() => isActive = false;

    #endregion

    #region ????? ---------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    #endregion
}
