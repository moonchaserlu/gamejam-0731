using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [Tooltip("Hour sequence for the clock puzzle")]
    public int[] hourCycle = new int[] { 1, 21, 13 };

    [Header("Game Over")]
    public string gameOverSceneName = "GameOver";

    private int[] recordedMinutes = new int[3];  // Recorded minutes for each cycle
    private int currentCycle = 0;                // Current cycle index
    private bool isActive = false;
    private bool isSolved = false;

    private void Awake()
    {
        if (submitButton != null) submitButton.onClick.AddListener(OnPasswordSubmit);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
        if (clearButton != null) clearButton.onClick.AddListener(ClearInput);
    }

    public override void Interact()
    {
        if (!isActive || isSolved) return;

        if (keypadUI != null)
        {
            keypadUI.SetActive(true);
            SetUIVisibility(true);

            if (passwordInput != null)
            {
                passwordInput.text = "";
                passwordInput.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(passwordInput.gameObject);
            }
        }
    }

    public void OnPasswordSubmit()
    {
        if (!isActive || isSolved) return;

        string correct = $"{recordedMinutes[0]:D2}{recordedMinutes[1]:D2}{recordedMinutes[2]:D2}";

        if (passwordInput != null && passwordInput.text == correct)
        {
            isSolved = true;
            CloseKeypad();

            if (!string.IsNullOrEmpty(gameOverSceneName))
                SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            // Play error sound or effect here if needed
            passwordInput.text = "";
        }
    }

    public void OnCancel()
    {
        CloseKeypad();
    }

    public void ClearInput()
    {
        if (passwordInput != null)
        {
            passwordInput.text = "";
            passwordInput.ActivateInputField();
        }
    }

    private void CloseKeypad()
    {
        if (keypadUI != null)
        {
            keypadUI.SetActive(false);
            SetUIVisibility(false);
        }

        if (gameManager != null && gameManager.playerController != null)
            gameManager.playerController.EnableControls();
    }

    private void SetUIVisibility(bool visible)
    {
        if (submitButton != null) submitButton.gameObject.SetActive(visible);
        if (cancelButton != null) cancelButton.gameObject.SetActive(visible);
        if (clearButton != null) clearButton.gameObject.SetActive(visible);
        if (passwordInput != null) passwordInput.gameObject.SetActive(visible);
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

        CloseKeypad();
        if (clockDisplay != null) clockDisplay.text = "--:--";
    }

    private void UpdateClockDisplay(int hour, int minute)
    {
        if (clockDisplay != null)
            clockDisplay.text = $"{hour:D2}:{minute:D2}";
    }
}