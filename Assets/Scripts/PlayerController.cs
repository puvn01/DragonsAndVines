using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    //public int currentTileIndex = -1;
    //public int moveToIndex = -1;
    public string playerName = "Player";
    public float modifierMoveDelay = 0.15f;
    public bool isComputerPlayer = false;
    //public bool isReady = false;


    public GridController grid;
    public bool isMoveAllowed = false;



    private SpriteRenderer spriteRenderer;
    private List<TileController> tileList;
    private BeadController[] beads;
    public BeadController selectedBead;
    private int CurrentDiceRoll;
    private List<int> DiceRollHistory = new List<int>();

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        beads = GetComponentsInChildren<BeadController>();
        
    }

    private void Start()
    {
        tileList = grid._tilesList;
    }


    public bool GetSelectedBead()
    {
        if (isComputerPlayer)
        {
            Debug.Log("CPU Turn");
            //selectedBead = beads[UnityEngine.Random.Range(0, 3)];
            selectedBead = beads[0];
            selectedBead.isSelected = true;
            spriteRenderer = selectedBead.GetComponentInChildren<SpriteRenderer>();
            return true;
            
        }

        foreach (BeadController bead in beads)
        {
            if (bead.isSelected)
            {
                selectedBead = bead;
                spriteRenderer = selectedBead.GetComponentInChildren<SpriteRenderer>();
                //currentTileIndex = bead.currentTileIndex;
                //isReady = bead.isReady;
                Debug.Log("Selected Bead: " + bead.name);
                return true ;
            }
        }
        return false;
    }

    public void DeselectCurrentBead()
    {
        selectedBead.isSelected = false;
        selectedBead=null;
        Debug.Log("Bead Deselected");
    }

    //private void UpdateSelectedBead()
    //{
    //    selectedBead.currentTileIndex = currentTileIndex;
    //    selectedBead.moveToIndex = moveToIndex;
    //    selectedBead.isReady = isReady;
    //}

    public void JumpToCoord(Vector2 coordinates)
    {
        selectedBead.transform.position = coordinates;
        isMoveAllowed = false;
        //isReady = true;
        //UpdateSelectedBead();

    }



    // Smooth movement towards the target position
    public IEnumerator Move(int moves, System.Action checkMoveModifier, bool isModifiedMove=false)
    {
        int moveToIndex;
        int currentTileIndex = selectedBead.currentTileIndex;
        moveToIndex = currentTileIndex + moves;

        if (moveToIndex >= tileList.Count - 1)
            //Set to Max grid entry
            moveToIndex = tileList.Count - 1;


        isMoveAllowed = false;

        if (!isModifiedMove)
        {
            //Move to original target position
            while (currentTileIndex != moveToIndex)
            {
                currentTileIndex++;

                while (selectedBead.transform.position != tileList[currentTileIndex].transform.position)
                {
                    selectedBead.transform.position = Vector2.MoveTowards(selectedBead.transform.position, tileList[currentTileIndex].transform.position, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }

        }
        else
        {
            yield return new WaitForSeconds(modifierMoveDelay);
            while (selectedBead.transform.position != tileList[moveToIndex].transform.position)
            {
                selectedBead.transform.position = Vector2.MoveTowards(selectedBead.transform.position, tileList[moveToIndex].transform.position, moveSpeed * Time.deltaTime);
                yield return null;

            }
        }

        currentTileIndex = moveToIndex;
        selectedBead.currentTileIndex = currentTileIndex;
        selectedBead.moveToIndex = moveToIndex;

        isMoveAllowed = true;
        checkMoveModifier();

    }

    /// <summary>
    /// Scales the sprite of the Player Object
    /// </summary>
    /// <param name="scaleFactor">Percentage of scale to apply</param>
    public void Scale(Vector2 scaleFactor)
    {
        spriteRenderer.transform.localScale = scaleFactor;

    }

    public bool IsReady()
    {
        if (selectedBead is null)
        { 
            return false;
        }
        return selectedBead.isReady;
    }


    //Populates CurrentDice roll and adds to roll history history
    public void AddDiceRoll(int newDiceValue)
    {
        Debug.Log("CurrentDiceRoll: " + CurrentDiceRoll);
        CurrentDiceRoll = newDiceValue;
    }

    //Adds Dice Roll to history
    public void AddDiceRollHistory()
    {
        DiceRollHistory.Add(CurrentDiceRoll);
        CurrentDiceRoll = 0;

    }

    //Gets the current dice value
    public int GetCurrentDiceRoll()
    {
        return CurrentDiceRoll;
    }


    //Gets the current dice value
    public List<int> GetHistoryDiceRoll()
    {
        return DiceRollHistory;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Dragon"))
        //{
        //    DragonController dragon = collision.GetComponent<DragonController>();
        //    moveToIndex += dragon.moveModifier;
        //    Debug.Log(moveToIndex);
        //}
        //else if (collision.CompareTag("Vine"))
        //{
        //    Debug.Log("Vine Logic");
        //}
    }

}
