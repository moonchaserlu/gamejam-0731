using UnityEngine;

public class BoxPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class BoxPosition
    {
        public Transform box;
        public Transform targetPosition;
        public float positionTolerance = 0.2f;
        public float rotationTolerance = 15f;
    }

    public BoxPosition[] boxes;
    public CarriageLoopCoroutine loopSystem;
    public bool isActive = false;

    void Update()
    {
        if (!isActive) return;
        CheckSolution();
    }

    private void CheckSolution()
    {
        foreach (var box in boxes)
        {
            if (Vector2.Distance(box.box.position, box.targetPosition.position) > box.positionTolerance)
                return;

            if (Mathf.Abs(box.box.eulerAngles.z - box.targetPosition.eulerAngles.z) > box.rotationTolerance)
                return;
        }

        isActive = false;
        loopSystem.CompletePuzzle(1);
    }

    public void Activate() => isActive = true;
    public void ResetPuzzle() => isActive = false;
}