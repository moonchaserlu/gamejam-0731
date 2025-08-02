using System.Collections;
using UnityEngine;

public abstract class GameCoroutine : MonoBehaviour
{
    protected Coroutine currentRoutine;
    protected bool isRunning = false;

    protected virtual void Awake()
    {
        // 基类初始化代码
    }

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