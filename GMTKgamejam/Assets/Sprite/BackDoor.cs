using UnityEngine;

public class BackDoor : Door
{
    public static BackDoor Instance;

    [Header("Loop Hook")]
    [Tooltip("????? ClockPuzzle ????????????????")]
    public ClockPuzzle clockPuzzle;

    void Awake()
    {
        Instance = this;
    }

    // ????????????????
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player") && !isLocked)
        {
            // ????????????“??????”
            if (clockPuzzle != null)
            {
                clockPuzzle.OnPlayerEnterCarriage();
            }
        }
    }
}
