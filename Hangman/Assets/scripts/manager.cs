using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class manager : MonoBehaviour
{
    public GameObject UICanvas;
    public GameObject customButton;
    public GameObject customText;
    public GameObject hangman;
    public GameObject hiddenLettersGO;
    public GameObject alphabetGO;
    public string word;

    private wordDatabase db = new wordDatabase();
    private UIController UIControl;

    char[] slicedWord;
    int wordLength;
    int guessesLeft;
    int guessed;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    char[] alphRdy;
    List<GameObject> letters;
    List<GameObject> signs;
    int sWidth;
    int sHeight;
    float buttonSize;
    bool isLandscape;


    // Start is called before the first frame update
    void Start()
    {
        isLandscape = Screen.width > Screen.height ? true : false;
        UIControl = gameObject.GetComponent<UIController>();
        guessesLeft = 6;
        guessed = 0;
        letters = new List<GameObject>();
        signs = new List<GameObject>();
        sWidth = Screen.width;
        sHeight = Screen.height;
        initializeOnStart();
    }

    void Update()
    {


    }

    #region Orientation check + FixedUpdate()
    private void FixedUpdate()
    {
        CheckOrientation();
    }

    public void CheckOrientation()
    {
        if (Screen.width != sWidth && Screen.height != sHeight)
        {
            sWidth = Screen.width;
            sHeight = Screen.height;
            CalculateButtonSize();
            isLandscape = !isLandscape;
            RepositionButtons();
        }
    }

    #endregion
    
    void initializeOnStart()
    {
        db.loadEngDatabase();
        word = db.GetRandomWord();
        Slice(word);
        prepareAlphabet();
        offsetLetters();
    }

    #region StringOps 

    void Slice(string randomWord) {
        slicedWord = randomWord.ToCharArray();
        wordLength = slicedWord.Length;

        foreach (char letter in slicedWord) {
            GameObject txt = Instantiate(customText);
            txt.transform.SetParent(hiddenLettersGO.transform);
            txt.transform.GetComponent<Text>().text = "_";
            txt.name = letter.ToString().ToUpper();
            signs.Add(txt);
        }
    }

    void prepareAlphabet() {
        alphRdy = alphabet.ToCharArray();
        buttonSize = CalculateButtonSize();
        foreach (char letter in alphRdy) {
            GameObject btn = Instantiate(customButton);
            btn.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
            btn.GetComponent<Button>().onClick.AddListener(delegate {searchForLetter(btn.name, btn);});
            btn.transform.SetParent(alphabetGO.transform);
            btn.transform.GetChild(0).GetComponent<Text>().text = letter.ToString().ToUpper();
            btn.name = letter.ToString().ToUpper();
            letters.Add(btn);
        }
        RepositionButtons();
    }

    #endregion

    #region Screen Orientation
    float CalculateButtonSize() {
        return Screen.width / 20;
    }

    void RepositionButtons() {
        if (isLandscape)
        {
            MakeHorizontalOffset();
        }
        else {
            MakeVerticalOffset();
        }
    }
    #endregion

    #region Generated UI offsetting
    void MakeVerticalOffset() {
        float offset = buttonSize + 5f;
        GameObject startingPoint = new GameObject();
        startingPoint.name = "reference for offset";
        startingPoint.transform.SetParent(alphabetGO.transform);
        startingPoint.transform.localPosition = new Vector3(-5*offset, 0f, 0f);
        GameObject previous = startingPoint;
        int counter = 0;
        foreach (GameObject letter in letters)
        {
            if (counter == 9 || counter == 18)
            {
                previous = startingPoint;
            }
            if (counter < 9)
            {
                letter.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, 0f, 0f);
            }
            else if (counter > 9 && counter < 18)
            {
                letter.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, -offset, 0f);
            }
            else
            {
                letter.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, -offset * 2, 0f);
            }
            counter++;
            previous = letter;
        }
        alphabetGO.transform.localPosition = new Vector3(0f, -350f, 0f);
    }

    void MakeHorizontalOffset() {
        float offset = buttonSize + 5f;
        GameObject startingPoint = new GameObject();
        startingPoint.name = "reference for offset";
        startingPoint.transform.SetParent(alphabetGO.transform);
        startingPoint.transform.localPosition = new Vector3(-((14f*offset) /2), 0f, 0f);
        GameObject previous = startingPoint;
        int counter = 0;
        foreach (GameObject letter in letters) {
            if (counter == 13) {
                previous = startingPoint;
            }
            if (counter < 13)
            {
                letter.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, 0f, 0f);
            }
            else
            {
                letter.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, -offset, 0f);
            }
            counter++;
            previous = letter;
        }
        DestroyImmediate(startingPoint);
        alphabetGO.transform.localPosition = new Vector3(0f, -350f, 0f);
    }

    //offsetting hidden characters
    void offsetLetters()

    {
        float offset = buttonSize + 5f;
        GameObject startingPoint = new GameObject();
        startingPoint.name = "reference for offset";
        startingPoint.transform.SetParent(hiddenLettersGO.transform);
        float imageWidth = (wordLength * offset) + (offset * 4);
        hiddenLettersGO.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(imageWidth, 2 * offset);
        float letterPlayground = imageWidth / 10 * 7;
        offset = letterPlayground / wordLength;
        if (wordLength % 2 == 0) {
            startingPoint.transform.localPosition = new Vector3(-(offset * (wordLength * 0.5f)) - (offset * 0.5f), 0f, 0f);
        }
        else startingPoint.transform.localPosition = new Vector3(-(offset * (wordLength * 0.5f)) - offset, 0f, 0f);
        GameObject previous = startingPoint;

        foreach (GameObject sign in signs)
        {
            sign.transform.localPosition = new Vector3(previous.transform.localPosition.x + offset, 0f, 0f);
            sign.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
            previous = sign;
        }
        DestroyImmediate(startingPoint);
        hiddenLettersGO.transform.localPosition = new Vector3(0f, -256f, 0f);
    }

    private float calculateXAxisOffset() {
        return 105f * wordLength / 2;
    }

    #endregion

    #region Game Logic
    void searchForLetter(string letter, GameObject btn) {
        if (word.ToUpper().Contains(letter))
        {
            foreach (GameObject hiddenL in signs)
            {
                if (hiddenL.name == letter)
                {
                    hiddenL.GetComponent<Text>().text = letter;
                    guessed++;
                }
            }
        }
        else {
            hangman.transform.GetChild(6 - guessesLeft).gameObject.SetActive(true);
            guessesLeft--;
        }
        btn.SetActive(false);
        checkGameCondition();

    }

    void disableButtons() {
        foreach (GameObject btn in letters)
        {
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    void checkGameCondition() {
        if (guessesLeft <= 0)
        {
            UIControl.RoggleEndgameUI("loss", word);
            disableButtons();
        }
        else if (guessed == wordLength)
        {
            UIControl.RoggleEndgameUI("victory", word);
            disableButtons();
        }
    }

    public void PlayAgain() {
        SceneManager.LoadScene("Scene01");
    }

    public void WhatIsIt() {
        Application.OpenURL("https://www.thefreedictionary.com/" + word);
    }

    #endregion
}


