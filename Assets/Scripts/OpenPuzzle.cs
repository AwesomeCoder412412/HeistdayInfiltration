using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPuzzle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonClicked()
    {
        boxCollider.enabled = false;
        puzzleCanvas.SetActive(true);
    }
    public BoxCollider boxCollider;
    public GameObject puzzleCanvas;
    
}
