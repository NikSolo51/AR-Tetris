using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePoints : MonoBehaviour
{
    private Text _text;
    private int points;
    
    private void OnEnable()
    {
        Board.OnClearLine += UpdateText;
    }

    private void OnDisable()
    {
        Board.OnClearLine -= UpdateText;
    }
    
    private void Start()
    {
        _text = GetComponent<Text>();
    }

    void UpdateText()
    {
        points += 100;
        _text.text = Convert.ToString(points);
    }
}
