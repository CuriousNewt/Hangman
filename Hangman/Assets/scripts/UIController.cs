using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject endGamePanel, canvas;

    // Start is called before the first frame update
    void Start()
    {
        endGamePanel.SetActive(false);
    }

    public void RoggleEndgameUI(string condition, string word) {
        endGamePanel.SetActive(true);
        if (condition == "victory") {
            endGamePanel.transform.GetChild(0).GetComponent<Text>().text = "You got it right!";
        } else if (condition == "loss") {
            endGamePanel.transform.GetChild(0).GetComponent<Text>().text = "That didn't go well... Your word was " + word + " \nWant to try again?";
        }
    }

    public void SwitchToPortraitUI() {
        endGamePanel.transform.
    }
}
