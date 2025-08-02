using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarriageLoopCoroutine : GameCoroutine
{
    public enum GameState { Initializing, Puzzle1, Puzzle2, Puzzle3, FinalEscape }

    public static CarriageLoopCoroutine Instance { get; private set; }

    [Header("Level Roots")]
    public GameObject level1Root;
    public GameObject level2Root;
    public GameObject level3Root;

    [Header("References")]
    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public FrontDoor frontDoor;
    public BackDoor backDoor;

    [Header("Timing")]
    public float doorTransitionDelay = 0.2f;   

    [Header("Puzzle References")]
    public PosterPuzzle posterPuzzle;  
    public BoxPuzzle boxPuzzle;        
    public ClockPuzzle clockPuzzle;    

    [Header("Game Over")]
    public string gameOverSceneName = "GameOver"; 

    [Header("Debug Jump (Editor)")]
    public bool enableDebugHotkeys = true;   
    private bool pendingJump = false;        
    private int pendingTargetLevel = -1;     

    private GameState currentState = GameState.Initializing;
    private GameObject currentPlayer;
    private bool[] puzzleCompleted = new bool[3]; 

    
    private int currentLevelIndex = 0;

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        base.Awake();
    }
    protected override IEnumerator RunCoroutine()
    {
        while (true)
        {
            
            if (enableDebugHotkeys)
            {
                if (Input.GetKeyDown(KeyCode.F1)) { QueueJump(+1); }
                if (Input.GetKeyDown(KeyCode.F2)) { QueueJump(-1); }
            }

            switch (currentState)
            {
                case GameState.Initializing:
                    yield return InitializeGame();
                    break;

                case GameState.Puzzle1:
                    yield return HandlePuzzleLevel(0, GameState.Puzzle2);
                    break;

                case GameState.Puzzle2:
                    yield return HandlePuzzleLevel(1, GameState.Puzzle3);
                    break;

                case GameState.Puzzle3:
                    yield return HandlePuzzle3();
                    break;

                case GameState.FinalEscape:
                    yield return HandleFinalEscape();
                    break;
            }
            yield return null;
        }
    }

  
    private IEnumerator InitializeGame()
    {
        // 清除现有玩家
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            yield return null;
        }

        // 重置所有状态
        puzzleCompleted = new bool[3];
        currentLevelIndex = 0;

        // 激活第一关
        SetLevelActive(0);

        // 重置所有谜题
        posterPuzzle.ResetPuzzle();
        boxPuzzle.ResetPuzzle();
        clockPuzzle.ResetPuzzle();

        // 生成新玩家
        SpawnPlayer();

        // 设置初始门状态
        frontDoor.Lock();
        backDoor.Unlock();
        backDoor.ResetPassingFlag();

        // 激活第一个谜题
        posterPuzzle.Activate();

        currentState = GameState.Puzzle1;
    }

    private IEnumerator HandlePuzzleLevel(int levelIndex, GameState nextState)
    {
        
        backDoor.Unlock();
        backDoor.ResetPassingFlag();

        
        while (true)
        {
            if (backDoor.IsPlayerPassing)
            {
                backDoor.ResetPassingFlag();

                
                if (pendingJump && pendingTargetLevel >= 0)
                {
                    pendingJump = false;
                    yield return StartCoroutine(SwitchToLevelWithBlackout(pendingTargetLevel));
                    currentState = (GameState)(GameState.Puzzle1 + pendingTargetLevel);
                    yield break;
                }

                
                if (!puzzleCompleted[levelIndex])
                {
                    
                    yield return StartCoroutine(ReturnToFrontWithBlackout(1.0f));
                }
                else
                {
                    
                    int nextLevelIndex = levelIndex + 1;
                    yield return StartCoroutine(SwitchToLevelWithBlackout(nextLevelIndex));
                    currentLevelIndex = nextLevelIndex;
                    currentState = nextState;
                    yield break;
                }
            }
            yield return null;
        }
    }

    
    private IEnumerator HandlePuzzle3()
    {
        
        SetLevelActive(2);
        clockPuzzle.Activate();

        
        backDoor.Unlock();
        backDoor.ResetPassingFlag();

        
        while (!puzzleCompleted[2])
        {
            if (backDoor.IsPlayerPassing)
            {
                backDoor.ResetPassingFlag();
                
                yield return StartCoroutine(ReturnToFrontWithBlackout(1.0f));
            }
            yield return null;
        }

       
        yield return new WaitForSeconds(doorTransitionDelay);
        frontDoor.Unlock();

        currentState = GameState.FinalEscape;
    }

    private IEnumerator HandleFinalEscape()
    {
       
        while (!frontDoor.IsPlayerPassing) yield return null;

        
        frontDoor.ResetPassingFlag();
        yield return GameManager.Instance.BlackoutAndTeleport(playerSpawnPoint.position, 0.2f);

        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            GameManager.Instance.GameWin(); 
        }

        
        currentState = GameState.Initializing;
        yield return null;
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer);
        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        GameManager.Instance.playerController = currentPlayer.GetComponent<PlayerController>();
    }

   
    public void CompletePuzzle(int puzzleIndex)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzleCompleted.Length)
        {
            puzzleCompleted[puzzleIndex] = true;
            Debug.Log($"Puzzle {puzzleIndex + 1} Completed!");
        }
    }

    
    public void SetPuzzleSolved(int puzzleIndex, bool solved)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzleCompleted.Length)
        {
            puzzleCompleted[puzzleIndex] = solved;
        }
    }

    public void ResetToCurrentLevel()
    {
        StopCoroutineSystem();

        // 重置当前关卡的谜题
        switch (currentLevelIndex)
        {
            case 0:
                posterPuzzle.ResetPuzzle();
                posterPuzzle.Activate();
                break;
            case 1:
                boxPuzzle.ResetPuzzle();
                boxPuzzle.Activate();
                break;
            case 2:
                clockPuzzle.ResetPuzzle();
                clockPuzzle.Activate();
                break;
        }

        // 重置玩家位置
        if (currentPlayer != null)
        {
            currentPlayer.transform.position = playerSpawnPoint.position;
            var playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null) playerController.ResetPlayer();
        }

        // 重置门状态
        frontDoor.Lock();
        backDoor.Unlock();
        backDoor.ResetPassingFlag();

        // 重置谜题完成状态
        puzzleCompleted[currentLevelIndex] = false;

        StartCoroutineSystem();
    }
    private IEnumerator SwitchToLevelWithBlackout(int targetLevelIndex)
    {
        
        yield return GameManager.Instance.BlackoutAndTeleport(playerSpawnPoint.position, 0.25f);

        
        SetLevelActive(targetLevelIndex);

        
        switch (targetLevelIndex)
        {
            case 0:
                posterPuzzle.Activate();
                boxPuzzle.ResetPuzzle();
                //clockPuzzle.ResetPuzzle();
                break;
            case 1:
                posterPuzzle.ResetPuzzle();
                boxPuzzle.Activate();
                //clockPuzzle.ResetPuzzle();
                break;
            case 2:
                posterPuzzle.ResetPuzzle();
                boxPuzzle.ResetPuzzle();
                clockPuzzle.Activate();
                break;
        }

        
        yield return new WaitForSeconds(doorTransitionDelay);
    }

    
    private IEnumerator ReturnToFrontWithBlackout(float holdSeconds)
    {
        yield return GameManager.Instance.BlackoutAndTeleport(
            targetPosition: playerSpawnPoint.position,
            holdSeconds: holdSeconds
        );
    }

    
    private void SetLevelActive(int levelIndex)
    {
        currentLevelIndex = Mathf.Clamp(levelIndex, 0, 2);
        if (level1Root) level1Root.SetActive(currentLevelIndex == 0);
        if (level2Root) level2Root.SetActive(currentLevelIndex == 1);
        if (level3Root) level3Root.SetActive(currentLevelIndex == 2);
    }

    
    private void QueueJump(int delta)
    {
        int target = Mathf.Clamp(currentLevelIndex + delta, 0, 2);
        pendingJump = true;
        pendingTargetLevel = target;
        Debug.Log($"[DEV] Next entry will jump to Level {pendingTargetLevel + 1}");
    }
}


