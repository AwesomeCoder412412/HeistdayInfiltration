using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPuzzle : MonoBehaviour
{
    public Button firstButton;
    public Button[] buttons;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button correctButton;
    private int randomButton;
    private bool solved = false;
    private void OnEnable()
    {
        if (!solved)
        {
            string numbers = "" + (int)Random.Range(1000, 9999);
            randomButton = Random.Range(0, buttons.Length - 1);
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == randomButton)
                {
                    buttons[i].GetComponentInChildren<Text>().text = numbers;
                }
                else
                {
                    buttons[i].GetComponentInChildren<Text>().text = "" + (int)Random.Range(1000, 9999);
                }
            }
            firstButton.GetComponentInChildren<Text>().text = numbers;
            StartCoroutine(HideButton());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private IEnumerator HideButton()
    {
        yield return new WaitForSeconds(3);
        firstButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }
    public void ClickedAnswer(int button)
    {
        if (button == randomButton + 1)
        {
            gameObject.SetActive(false);
            solved = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            ResetPuzzle();
        }
    }
    public void ResetPuzzle()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        firstButton.gameObject.SetActive(true);
        //StartCoroutine(HideButton());
        OnEnable();
    }

}
