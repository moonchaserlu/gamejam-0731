using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI gameoverText;
    public GameObject restartPanel;

    private void Start()
    {
        gameoverText.text = "";
        restartPanel.SetActive(false);
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        string[] fixedHours = { "01", "21", "13" };
        int[] minutes = GameData.finalMinutes;

        yield return new WaitForSeconds(1.2f);

        
        for (int i = 0; i < 3; i++)
        {
            gameoverText.text = fixedHours[i] + ":" + minutes[i].ToString("D2");
            yield return new WaitForSeconds(1.2f);
        }

        
        float delay = 0.8f;
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < 2; k++)
            {
                int hour = Random.Range(0, 24);
                int minute = Random.Range(0, 60);
                gameoverText.text = hour.ToString("D2") + ":" + minute.ToString("D2");
                yield return new WaitForSeconds(delay);
            }
            delay = Mathf.Max(delay - 0.2f, 1f);
        }

        
        gameoverText.text = "";
        yield return new WaitForSeconds(1.5f);
        restartPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }
    public void ReturnMeun()
    {
        SceneManager.LoadScene("GameStart"); 
    }
}

