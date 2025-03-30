using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI message;



    private void Awake()
    {
        instance = this;
        //diceText = GetComponentInChildren<TextMeshProUGUI>();
        
    }

    public void GetDiceRoll(int value)
    {
        diceText.text = value.ToString();

    }

    public void GameOver(bool hasWon, string winMsg = "You Win!!")
    {
        //Color winTextColor = new Color();
        if (hasWon)
        {
            //winText.color = winTextColor;
            winText.text = winMsg;
            winText.gameObject.SetActive(true);
        }
        

    }

    public void GeneralMessage(string msg)
    {
        message.text = msg;
    }
}
