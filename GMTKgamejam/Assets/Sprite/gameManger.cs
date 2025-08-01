using System.Collections;
using UnityEngine;
using UnityEngine.UI;   // ???????????? Image??????

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Loop System")]
    public CarriageLoopCoroutine loopSystem;   // ? Inspector ???

    [Header("Player")]
    public PlayerController playerController;  // ???? CarriageLoopCoroutine.SpawnPlayer ??

    [Header("Black Overlay (UI)")]
    [Tooltip("???????? CanvasGroup?????? Image????????/??")]
    public CanvasGroup blackOverlay;
    [Tooltip("????/???????")]
    public float fadeDuration = 0.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // ?????? —— ???????? GameCoroutine ????
        if (loopSystem != null)
            loopSystem.StartCoroutineSystem();

        // ???????????????????????? SpawnPlayer ???????
        if (playerController != null)
            playerController.EnableControls();
    }

    /// <summary>
    /// ????????????/?????
    /// </summary>
    public void GameWin()
    {
        Debug.Log("Game Win!");
        if (playerController != null)
            playerController.DisableControls();
        // TODO: ???????????/??????
    }

    /// <summary>
    /// ??????????????
    /// </summary>
    public void ResetGame()
    {
        StopAllCoroutines();

        if (loopSystem != null)
        {
            // ??? GameCoroutine ????
            loopSystem.StopCoroutineSystem();
            loopSystem.StartCoroutineSystem();
        }

        // ?????? PlayerController.ResetPlayer?????????????
        // if (playerController != null) playerController.ResetPlayer();
    }

    /// <summary>
    /// ????????? ? ???? ? ??? targetPosition???????
    /// ? ???? holdSeconds ? ?? ? ????
    /// </summary>
    public IEnumerator BlackoutAndTeleport(Vector3 targetPosition, float holdSeconds)
    {
        if (playerController != null)
            playerController.DisableControls();

        // ??
        yield return StartCoroutine(FadeBlack(1f, fadeDuration));

        // ?? + ?????????????
        if (playerController != null)
        {
            var rb = playerController.GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = Vector2.zero;
            playerController.transform.position = targetPosition;
        }

        // ????
        if (holdSeconds > 0f)
            yield return new WaitForSeconds(holdSeconds);

        // ??
        yield return StartCoroutine(FadeBlack(0f, fadeDuration));

        if (playerController != null)
            playerController.EnableControls();
    }

    /// <summary>
    /// ????/????? alpha?0=???1=???
    /// </summary>
    private IEnumerator FadeBlack(float targetAlpha, float duration)
    {
        if (blackOverlay == null)
        {
            // ?????????????
            yield break;
        }

        if (duration <= 0f)
        {
            blackOverlay.alpha = targetAlpha;
            yield break;
        }

        float start = blackOverlay.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            blackOverlay.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        blackOverlay.alpha = targetAlpha;
    }
}
