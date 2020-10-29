using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPuzzle : MonoBehaviour
{
    public bool didPuzzle = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !didPuzzle || other.gameObject.CompareTag("Tank") && !didPuzzle)
        {
            Cursor.visible = true;
            ScoreManager.score += 50;
            boxCollider.enabled = false;
            puzzleCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            PlayerController.instance.roomIndex++;
            didPuzzle = true;
        }
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
