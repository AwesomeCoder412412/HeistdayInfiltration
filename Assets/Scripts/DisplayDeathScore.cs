using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDeathScore : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    private string highScore = "highScore";
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetFloat(highScore) < ScoreManager.score)
        {
            PlayerPrefs.SetFloat(highScore, ScoreManager.score);
        }
        scoreText.text = "Score: " + ScoreManager.score;
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat(highScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
