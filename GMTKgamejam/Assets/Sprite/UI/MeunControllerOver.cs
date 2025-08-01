using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSequence : MonoBehaviour
{
    public TextMeshProUGUI clockDisplay;
    public TextMeshProUGUI finalText;
    public Button restartButton;
    public CanvasGroup fadeCanvas;

    private readonly int[] fixedHours = { 1, 21, 13 };
    public int[] recordedMinutes = new int[3];

    private void Start()
    {
        finalText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // ? GameData ?????????????
        if (GameData.finalMinutes.Length == 3)
        {
            recordedMinutes = GameData.finalMinutes;
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // ????
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FadeCanvasOut(fadeCanvas, 1f));
            yield return new WaitForSeconds(1f);
        }

        // ???????? + ??
        for (int i = 0; i < 3; i++)
        {
            clockDisplay.text = $"{fixedHours[i]:D2}:??";
            yield return new WaitForSeconds(1f);
        }

        // ????????
        float delay = 0.8f;
        for (int j = 0; j < 6; j++)
        {
            int hour = fixedHours[Random.Range(0, 3)];
            int minute = Random.Range(0, 60);
            clockDisplay.text = $"{hour:D2}:{minute:D2}";
            yield return new WaitForSeconds(delay);
            delay = Mathf.Max(0.4f, delay - 0.08f);
        }

        // ????????
        clockDisplay.text =
            $"{fixedHours[0]:D2}:{recordedMinutes[0]:D2}\n" +
            $"{fixedHours[1]:D2}:{recordedMinutes[1]:D2}\n" +
            $"{fixedHours[2]:D2}:{recordedMinutes[2]:D2}";

        yield return new WaitForSeconds(1f);

        // ???????
        finalText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    private IEnumerator FadeCanvasOut(CanvasGroup canvas, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            canvas.alpha = Mathf.Lerp(1f, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvas.alpha = 0f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene"); // ??????????
    }
}

