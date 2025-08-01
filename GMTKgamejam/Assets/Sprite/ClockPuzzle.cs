using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ClockPuzzle : Interactable
{
    [Header("UI")]
    public TextMeshPro clockDisplay;
    public GameObject keypadUI;
    public TMP_InputField passwordInput;
    public Button submitButton;
    public Button cancelButton;
    public Button clearButton;

    [Header("Flow")]
    public GameManager gameManager;

    [Header("Hour Cycle")]
    private int[] hourCycle = new int[] { 1, 21, 13 };
    private int[] recordedMinutes = new int[3];  // ???????????

    [Header("Game Over")]
    public string gameOverSceneName = "GameOver";
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1f;

    private int currentIndex = 0;
    private bool isActive = false;
    private bool isSolved = false;

    private void Awake()
    {
        submitButton.onClick.AddListener(OnPasswordSubmit);
        cancelButton.onClick.AddListener(OnCancel);
        clearButton.onClick.AddListener(ClearInput);
    }

    public override void Interact()
    {
        if (!isActive || isSolved) return;

        keypadUI.SetActive(true);
        passwordInput.text = "";
        passwordInput.ActivateInputField();
        EventSystem.current.SetSelectedGameObject(passwordInput.gameObject);
    }

    public void Activate()
    {
        isActive = true;
        isSolved = false;
        currentIndex = 0;

        // ???3???????????
        for (int i = 0; i < 3; i++)
        {
            recordedMinutes[i] = Random.Range(0, 60);
        }

        ShowNextTime();
    }

    public void OnPlayerEnterCarriage()
    {
        if (!isActive || isSolved) return;
        ShowNextTime();
    }

    private void ShowNextTime()
    {
        int hour = hourCycle[currentIndex % 3];
        int minute = recordedMinutes[currentIndex % 3];
        UpdateClockDisplay(hour, minute);

        currentIndex++;
    }

    public void OnPasswordSubmit()
    {
        if (isSolved) return;

        string correct = $"{recordedMinutes[0]:D2}{recordedMinutes[1]:D2}{recordedMinutes[2]:D2}";
        if (passwordInput.text == correct)
        {
            isSolved = true;
            GameData.finalMinutes = recordedMinutes;
            StartCoroutine(FadeAndLoadGameOver());
        }
        else
        {
            passwordInput.text = "";
        }
    }

    IEnumerator FadeAndLoadGameOver()
    {
        if (fadeGroup != null)
        {
            fadeGroup.gameObject.SetActive(true);
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
        }

        SceneManager.LoadScene(gameOverSceneName);
    }

    public void OnCancel() => keypadUI.SetActive(false);

    public void ClearInput()
    {
        passwordInput.text = "";
        passwordInput.ActivateInputField();
    }

    private void UpdateClockDisplay(int hour, int minute)
    {
        if (clockDisplay != null)
            clockDisplay.text = $"{hour:D2}:{minute:D2}";
    }
}
