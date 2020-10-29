using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonSaysRipoff : MonoBehaviour
{

    private List<int> clickedButtons = new List<int>();
    public GameObject flashButtonParent, buttonParent;
    public Button[] flashButtons;
    public Button[] buttons;
    private bool solved = false;
    private List<int> uninteractableButtons = new List<int>();
    private void OnEnable()
    {
        if (!solved)
        {
            for (int i = 0; i < flashButtons.Length; i++)
            {
                if (Random.Range(0f, 1f) < .33f)
                {
                    flashButtons[i].interactable = false;
                    uninteractableButtons.Add(i);
                    Debug.Log("uninteractable" + i);
                }
            }
            StartCoroutine(HideButtons());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private IEnumerator HideButtons()
    {
        yield return new WaitForSeconds(3);
        flashButtonParent.SetActive(false);
        yield return new WaitForSeconds(3);
        buttonParent.SetActive(true);
    }
    public void ClickedButton(int button)
    {
        Debug.Log(button);
       if (uninteractableButtons.Contains(button))
        {
            buttons[button].interactable = false;
            clickedButtons.Add(button);
            if (clickedButtons.Count == uninteractableButtons.Count)
            {
                gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
       else
        {
            ResetPuzzle();
        }
    }
    public void ResetPuzzle()
    {
        buttonParent.SetActive(false);
        
        flashButtonParent.SetActive(true);
        foreach (Button button in flashButtons)
        {
            button.interactable = true;
        }
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        uninteractableButtons = new List<int>();
        clickedButtons = new List<int>();
        OnEnable();
    }
    private IEnumerator ThreeSeconds()
    {
        buttonParent.SetActive(false);
        yield return new WaitForSeconds(3);
        flashButtonParent.SetActive(true);
        foreach (Button button in flashButtons)
        {
            button.interactable = true;
        }
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        uninteractableButtons = new List<int>();
        clickedButtons = new List<int>();
        yield return new WaitForSeconds(3);
    }

}

