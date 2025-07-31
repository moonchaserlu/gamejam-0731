using System.Collections;
using UnityEngine;

public abstract class GameCoroutine : MonoBehaviour
{
    protected Coroutine currentRoutine;
    protected bool isRunning = false;

    public void StartCoroutineSystem()
    {
        if (!isRunning)
        {
            isRunning = true;
            currentRoutine = StartCoroutine(RunCoroutine());
        }
    }

    public void StopCoroutineSystem()
    {
        if (isRunning)
        {
            StopCoroutine(currentRoutine);
            isRunning = false;
        }
    }

    protected abstract IEnumerator RunCoroutine();

    void OnDisable() => StopCoroutineSystem();
}