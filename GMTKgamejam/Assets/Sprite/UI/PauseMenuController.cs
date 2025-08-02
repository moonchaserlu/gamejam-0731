using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button restartLevelButton;
    public Button restartGameButton;
    public Button quitButton;

    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        restartLevelButton.onClick.AddListener(RestartCurrentLevel);
        restartGameButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        PlayButtonSound();
        GameManager.Instance?.playerController?.DisableControls();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        PlayButtonSound();
        GameManager.Instance?.playerController?.EnableControls();
    }

    public void RestartCurrentLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        CarriageLoopCoroutine.Instance?.ResetToCurrentLevel();
    }

    public void RestartGame()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    private void PlayButtonSound()
    {
        if (uiAudioSource && buttonClickSound)
            uiAudioSource.PlayOneShot(buttonClickSound);
    }
}