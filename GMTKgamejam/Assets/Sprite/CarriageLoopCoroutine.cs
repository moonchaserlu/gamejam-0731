using System.Collections;
using UnityEngine;

public class CarriageLoopCoroutine : GameCoroutine
{
    public enum GameState { Initializing, Puzzle1, Puzzle2, Puzzle3, FinalEscape }

    [Header("References")]
    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public FrontDoor frontDoor;
    public BackDoor backDoor;

    [Header("Puzzle Settings")]
    public float respawnDelay = 1.5f;
    public float doorTransitionDelay = 0.8f;

    [Header("Puzzle References")]
    public PosterPuzzle posterPuzzle;
    public BoxPuzzle boxPuzzle;
    public ClockPuzzle clockPuzzle;

    private GameState currentState = GameState.Initializing;
    private GameObject currentPlayer;
    private bool[] puzzleCompleted = new bool[3];

    protected override IEnumerator RunCoroutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case GameState.Initializing:
                    yield return InitializeGame();
                    break;

                case GameState.Puzzle1:
                    yield return HandlePuzzle1();
                    break;

                case GameState.Puzzle2:
                    yield return HandlePuzzle2();
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
        posterPuzzle.ResetPuzzle();
        boxPuzzle.ResetPuzzle();
        clockPuzzle.ResetPuzzle();

        SpawnPlayer();

        puzzleCompleted = new bool[3];
        frontDoor.Lock();
        backDoor.Lock();

        currentState = GameState.Puzzle1;
        yield return null;
    }

    private IEnumerator HandlePuzzle1()
    {
        posterPuzzle.Activate();

        while (!puzzleCompleted[0]) yield return null;

        yield return new WaitForSeconds(doorTransitionDelay);
        backDoor.Unlock();

        while (!backDoor.IsPlayerPassing) yield return null;

        currentState = GameState.Puzzle2;
    }

    private IEnumerator HandlePuzzle2()
    {
        backDoor.Lock();
        boxPuzzle.Activate();

        while (!puzzleCompleted[1]) yield return null;

        yield return new WaitForSeconds(doorTransitionDelay);
        backDoor.Unlock();

        while (!backDoor.IsPlayerPassing) yield return null;

        currentState = GameState.Puzzle3;
    }

    private IEnumerator HandlePuzzle3()
    {
        backDoor.Lock();
        clockPuzzle.Activate();

        while (!puzzleCompleted[2]) yield return null;

        yield return new WaitForSeconds(doorTransitionDelay);
        frontDoor.Unlock();

        currentState = GameState.FinalEscape;
    }

    private IEnumerator HandleFinalEscape()
    {
        while (!frontDoor.IsPlayerPassing) yield return null;
        GameManager.Instance.GameWin();
        currentState = GameState.Initializing;
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
            Debug.Log($"ÃÕÌâ {puzzleIndex + 1} Íê³É!");
        }
    }
}