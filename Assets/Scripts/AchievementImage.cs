using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementImage : MonoBehaviour
{
    public AchievementType achievement;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(PlayerPrefs.GetInt("Achievement" + achievement) == 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public enum AchievementType
{
    GoingSolo,
    LivingOnTheEdge,
    ArnoldSchwarzenegger
}