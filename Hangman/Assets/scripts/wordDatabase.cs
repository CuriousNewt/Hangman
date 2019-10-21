using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wordDatabase
{
    private string[] words;

    public void loadEngDatabase()
    {
        words = System.IO.File.ReadAllLines("Assets/database/eng/words.txt");
    }

    public void loadCzDatabase()
    {
        words = System.IO.File.ReadAllLines("Assets/database/cz/words.txt");
    }

    public string GetRandomWord() {
        return words[Random.Range(0, words.Length)];
    }
}
