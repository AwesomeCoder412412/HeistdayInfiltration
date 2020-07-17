﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;
    public List<GameObject> hearts;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already a instance of the HealthManager script");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            GameObject lastHeart = hearts[hearts.Count - 1];
            Destroy(lastHeart);
            hearts.Remove(lastHeart);

            if (hearts.Count == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        
    }
}
