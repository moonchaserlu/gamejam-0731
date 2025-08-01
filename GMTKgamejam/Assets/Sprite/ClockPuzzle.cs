using TMPro;
using UnityEngine;

public class ClockPuzzle : Interactable
{
    public TextMeshPro clockDisplay;
    public GameObject keypadUI;
    public TMP_InputField passwordInput;
    public CarriageLoopCoroutine loopSystem;
    public GameManager gameManager;

    private int[] recordedMinutes = new int[3];
    private int currentCycle = 0;   // ??“??????/??”???
    private bool isActive = false;
    private bool isSolved = false;

    public override void Interact()
    {
        if (!isActive || isSolved) return;
        keypadUI.SetActive(true);
        gameManager.playerController.DisableControls();
    }

    public void OnPasswordSubmit()
    {
        string correctPassword = $"{recordedMinutes[0]:D2}{recordedMinutes[1]:D2}{recordedMinutes[2]:D2}";

        if (passwordInput.text == correctPassword)
        {
            isSolved = true;
            keypadUI.SetActive(false);
            gameManager.playerController.EnableControls();
            loopSystem.CompletePuzzle(2);
        }
        else
        {
            passwordInput.text = "";
        }
    }

    /// <summary>
    /// ??“????????”????BackDoor.OnTriggerEnter2D ????
    /// </summary>
    public void OnPlayerEnterCarriage()
    {
        if (!isActive || isSolved || currentCycle >= 3) return;

        recordedMinutes[currentCycle] = Random.Range(0, 60);
        UpdateClockDisplay(recordedMinutes[currentCycle]);
        currentCycle++;
    }

    /// <summary>
    /// ?????????????????????? xx:00 ???
    /// ???2?3?? OnPlayerEnterCarriage ???
    /// </summary>
    public void Activate()
    {
        isActive = true;
        isSolved = false;
        currentCycle = 0;
        recordedMinutes = new int[3];

        recordedMinutes[0] = Random.Range(0, 60);
        UpdateClockDisplay(recordedMinutes[0]);
        currentCycle = 1; // ???????????2???
    }

    public void ResetPuzzle()
    {
        isActive = false;
        isSolved = false;
        currentCycle = 0;
        keypadUI.SetActive(false);
        // ???clockDisplay.text = "--:--";
    }

    private void UpdateClockDisplay(int minuteToShow)
    {
        int hour = Random.Range(0, 24);
        clockDisplay.text = $"{hour:D2}:{minuteToShow:D2}";
    }
}
