using UnityEngine;

public class BoxPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class BoxPosition
    {
        public Transform box;
        public Transform targetPosition;

        [Header("Tolerances")]
        
        public float positionTolerance = 0.30f;

        
        public float rotationTolerance = 15f;
    }

    [Header("Mappings")]
    public BoxPosition[] boxes;

    [Header("Flow")]
    public CarriageLoopCoroutine loopSystem;   

    [Header("UI Hint (Optional)")]
    
    public GameObject spaceHintUI;             

    [Header("Active")]
    public bool isActive = false;

    
    private bool lastSolvedState = false;       

    void OnEnable()
    {
        if (spaceHintUI != null) spaceHintUI.SetActive(false);
        
        ReportSolved(false);
    }

    void OnDisable()
    {
        
        ReportSolved(false);
    }

    void Update()
    {
        if (!isActive) return;

        bool solved = ComputeSolved();

        
        if (solved != lastSolvedState)
        {
            lastSolvedState = solved;
            
        }

       
        ReportSolved(solved);
    }

   
    public void Activate()
    {
        isActive = true;
        lastSolvedState = false;
        if (spaceHintUI != null) spaceHintUI.SetActive(true);
        ReportSolved(false); 
    }

    
    public void ResetPuzzle()
    {
        isActive = false;
        lastSolvedState = false;
        if (spaceHintUI != null) spaceHintUI.SetActive(false);
        ReportSolved(false);
    }

   
    private bool ComputeSolved()
    {
        if (boxes == null || boxes.Length == 0) return false;

        for (int i = 0; i < boxes.Length; i++)
        {
            var b = boxes[i];
            if (b.box == null || b.targetPosition == null) return false;

           
            float posErr = Vector2.Distance(b.box.position, b.targetPosition.position);
            if (posErr > b.positionTolerance) return false;

           
            float rotErr = Mathf.Abs(Mathf.DeltaAngle(b.box.eulerAngles.z, b.targetPosition.eulerAngles.z));
            if (rotErr > b.rotationTolerance) return false;
        }

        return true;
    }

    
    private void ReportSolved(bool solved)
    {
        if (loopSystem != null)
        {
           
            loopSystem.SetPuzzleSolved(1, solved);
        }
    }
}
