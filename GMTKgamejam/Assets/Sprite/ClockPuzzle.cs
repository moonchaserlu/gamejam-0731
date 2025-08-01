using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ClockPuzzle : Interactable
{
    [Header("UI")]
    public TextMeshPro clockDisplay;
    public GameObject keypadUI;
    public TMP_InputField passwordInput;

    [Header("Flow")]
    public GameManager gameManager;

    [Header("Hour Cycle")]
    [Tooltip("???????????")]
    public int[] hourCycle = new int[] { 1, 21, 13 };

    [Header("Game Over")]
    public string gameOverSceneName = "GameOver";

    // —— ???? —— //
    private int[] recordedMinutes = new int[3];  // ?????
    private int currentCycle = 0;                // ????????
    private bool isActive = false;
    private bool isSolved = false;

    public override void Interact()
    {
        if (!isActive || isSolved) return;

        if (keypadUI != null)
        {
            keypadUI.SetActive(true);

            // ? ?? TMP_InputField ??????????
            if (passwordInput != null)
            {
                passwordInput.text = "";
                passwordInput.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(passwordInput.gameObject);
            }
        }

        // ? ??????????? UI ????
        // if (gameManager != null && gameManager.playerController != null)
        //     gameManager.playerController.DisableControls();
    }

    public void OnPasswordSubmit()
    {
        if (!isActive || isSolved) return;

        string correct = $"{recordedMinutes[0]:D2}{recordedMinutes[1]:D2}{recordedMinutes[2]:D2}";

        if (passwordInput != null && passwordInput.text == correct)
        {
            isSolved = true;

            // ????? UI?????
            if (keypadUI != null) keypadUI.SetActive(false);
            if (gameManager != null && gameManager.playerController != null)
                gameManager.playerController.EnableControls();

            if (!string.IsNullOrEmpty(gameOverSceneName))
                SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            // ??????????
            if (passwordInput != null) passwordInput.text = "";
        }
    }

    public void OnPlayerEnterCarriage()
    {
        if (!isActive || isSolved) return;

        int hour = hourCycle[currentCycle % hourCycle.Length];
        int minute = Random.Range(0, 60);
        UpdateClockDisplay(hour, minute);

        if (currentCycle < 3)
        {
            recordedMinutes[currentCycle] = minute;
        }

        currentCycle++;
    }

    public void Activate()
    {
        isActive = true;
        isSolved = false;
        currentCycle = 0;

        int hour = hourCycle[0];
        int minute = Random.Range(0, 60);
        recordedMinutes[0] = minute;
        UpdateClockDisplay(hour, minute);
        currentCycle = 1;
    }

    public void ResetPuzzle()
    {
        isActive = false;
        isSolved = false;
        currentCycle = 0;

        if (keypadUI != null) keypadUI.SetActive(false);
        if (clockDisplay != null) clockDisplay.text = "--:--";
    }

    private void UpdateClockDisplay(int hour, int minute)
    {
        if (clockDisplay != null)
            clockDisplay.text = $"{hour:D2}:{minute:D2}";
    }
}

