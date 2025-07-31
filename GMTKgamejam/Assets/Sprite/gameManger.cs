using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CarriageLoopCoroutine loopSystem;
    public PlayerController playerController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() => InitializeGame();

    public void InitializeGame()
    {
        loopSystem.StartCoroutineSystem();
        playerController.EnableControls();
    }

    public void GameOver() => playerController.DisableControls();
    public void GameWin() => playerController.DisableControls();

    public void ResetGame()
    {
        loopSystem.StopCoroutineSystem();
        loopSystem.StartCoroutineSystem();
        playerController.ResetPlayer();
    }
}