using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static float score = 0f;
    public string highScore = "highScore";
    private void Start()
    {
        PlayerPrefs.SetFloat(highScore, 0);
        score = 0f;
    }
}
