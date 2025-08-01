using UnityEngine;

public class BoxPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class BoxPosition
    {
        public Transform box;
        public Transform targetPosition;
        [Header("Tolerances")]
        public float positionTolerance = 0.25f;
        public float rotationTolerance = 15f;
    }

    [Header("Mappings")]
    public BoxPosition[] boxes;

    [Header("Flow")]
    public CarriageLoopCoroutine loopSystem;

    [Header("UI Hint (Optional)")]
    [Tooltip("????????????????? Space ???????")]
    public GameObject spaceHintUI;   // ???? Activate/Reset ???

    [Header("Active")]
    public bool isActive = false;

    void OnEnable()
    {
        if (spaceHintUI != null) spaceHintUI.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        bool solved = true;
        for (int i = 0; i < boxes.Length; i++)
        {
            var b = boxes[i];
            if (b.box == null || b.targetPosition == null) { solved = false; break; }

            // ????
            float posErr = Vector2.Distance(b.box.position, b.targetPosition.position);
            if (posErr > b.positionTolerance) { solved = false; break; }

            // ?????? DeltaAngle ???
            float rotErr = Mathf.Abs(Mathf.DeltaAngle(b.box.eulerAngles.z, b.targetPosition.eulerAngles.z));
            if (rotErr > b.rotationTolerance) { solved = false; break; }
        }

        // ??????????????????
        if (solved)
        {
            isActive = false;
            if (spaceHintUI != null) spaceHintUI.SetActive(false);
            if (loopSystem != null) loopSystem.CompletePuzzle(1);
        }
    }

    public void Activate()
    {
        isActive = true;
        if (spaceHintUI != null) spaceHintUI.SetActive(true);
    }

    public void ResetPuzzle()
    {
        isActive = false;
        if (spaceHintUI != null) spaceHintUI.SetActive(false);
    }
}
