using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeunManagerStart : MonoBehaviour
{
    public GameObject creditPeanl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void closePanel()
    {
        creditPeanl.SetActive(false);
    }

    public void openPanel()
    {
        creditPeanl.SetActive(true);
    }

}
