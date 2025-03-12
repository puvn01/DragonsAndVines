using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public int diceValue=0;
    public int playerTurn = 0;
    public bool isDiceRolled = false;

    private void OnMouseDown()
    {
        if (!GameManager.instance.isGameOver)
        {
            Roll();
            isDiceRolled = true;
        }
        
        
    }

    public void Roll()
    {
        diceValue = Random.Range(1, 7);
        GameUI.instance.GetDiceRoll(diceValue);
        playerTurn = 1;
    }

 
}
