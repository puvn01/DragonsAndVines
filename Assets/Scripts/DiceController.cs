using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public int diceValue=0;
    public int playerTurn = 1;
    public bool isDiceRolled = false;


    private bool isDiceRolling = false;
    private void OnMouseDown()
    {
        Roll();
    }

    public void Roll()
    {
        if (!GameManager.instance.isGameOver && !isDiceRolled)
        {
            StartCoroutine("DiceRoll");
        }
    }

    public IEnumerator DiceRoll()
    {
        if (!isDiceRolling)
        {
            isDiceRolling = true;
            //Debug.Log("Rolling dice");
            yield return new WaitForSeconds(0.01f);
            diceValue = Random.Range(1, 7);
            GameUI.instance.GetDiceRoll(diceValue);           
            isDiceRolling = false;
            isDiceRolled = !isDiceRolling;
        }

        


    }
 
}
