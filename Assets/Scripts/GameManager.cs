using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DiceController dice;
    public GridManager grid;
    public PlayerController player;



    public bool isGameOver;
    private List<TileController> _tilesList;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _tilesList = grid._tilesList;
    }

    private void Update()
    {

        dice.transform.position = new Vector2(grid.transform.localScale.magnitude + 5, grid.transform.localScale.y / 2.0f);

        if (dice.playerTurn == 1)
        {
            if (dice.diceValue == 6 && player.currentTileIndex < 0)
                StartGame();
            else
            {
                MovePlayer();

            }
        }

    }

    private void StartGame()
    {
        if (dice == null || player == null)
            return;

        //Add player to board
        player.currentTileIndex = 0;
        TileController t = grid.GetTileAtIndex(0);
        player.Scale(grid.TileScaleFactor);
        Vector2 startPos = t.transform.position;
        player.Move(startPos);

        if (player.isMoveAllowed == false)
        {
            //reset dice turn
            dice.playerTurn = 0;
        }

    }



    private void MovePlayer()
    {

        if (player.currentTileIndex < 0)
        {
            return;
        }



        //get new position to move to
        //var playerMoveToIndex = player.currentTileIndex + dice.diceValue;
        var playerMoveToIndex = player.currentTileIndex + 1;


        if (playerMoveToIndex >= _tilesList.Count - 1)
        {
            //Set to Max grid entry
            playerMoveToIndex = _tilesList.Count - 1;

        }


        if (player.isMoveAllowed == false)
        {
            //reset dice turn
            dice.playerTurn = 0;
        }


        var moveModifier = GetMoveModifier(playerMoveToIndex);
        if (moveModifier != 0)
        {
            //add any move modifiers
            playerMoveToIndex += moveModifier;

        }

        //set the new coordinates for the player
        player.moveToIndex = playerMoveToIndex;

        //tell the player to move
        player.isMoveAllowed = true;

        if (player.currentTileIndex == _tilesList.Count - 1)
        {
            GameOver(true);
        }



    }

    
    private IEnumerator MoveModifier(int moveModifier)
    {
        
        //get move modifier
        //var moveModifier = GetMoveModifier(player.currentTileIndex);

        if (moveModifier != 0)
        {

            yield return new WaitForSeconds(0.02f);
            //add any move modifiers
            player.moveToIndex = player.currentTileIndex + moveModifier;

            dice.playerTurn = 1;
            //tell the player to move if there was a modifier
            player.isMoveAllowed = true;


        }
        else
        {
            yield return null;
        }


    }


    public void GameOver(bool hasWon)
    {
        //Win game
        GameUI.instance.GameOver(hasWon);
        this.isGameOver = true;
        Time.timeScale = 0.0f;
        //Show menu etc.
    }

    public int GetMoveModifier(int TileIndex) 
    {
        int modifier = 0;
        if (TileIndex >= _tilesList.Count - 1)
        {
           modifier= _tilesList.Count - 1;
        }
        
        //If Tile has a modifier then return modifier
        TileController t = grid.GetTileAtIndex(TileIndex);
        modifier = t.moveModifier;


        return modifier;
    }
        

}
