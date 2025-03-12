using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DiceController dice;
    public GridController grid;
    public PlayerController currentPlayer;



    public bool isGameOver;
    private List<TileController> _tilesList;
    private bool isMoveModified;
    private int playerTurn = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _tilesList = grid._tilesList;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Find all players
        currentPlayer = players[0].GetComponent<PlayerController>();
    }

    private void Update()
    {

        dice.transform.position = new Vector2(grid.transform.localScale.magnitude + 5, grid.transform.localScale.y / 2.0f);

        //if (dice.playerTurn == 1)
        //{
        //    if (dice.diceValue == 6 && player.currentTileIndex < 0)
        //        StartGame();
        //    else
        //    {
        //        //MovePlayer(1);

        //        //MoveModifier();

        //    }
        //}

        if (dice.playerTurn == 1)
        {
            if (dice.diceValue == 6 && currentPlayer.currentTileIndex < 0)
                AddCurrentPlayerToBoard();
            else
            {
                moveCurrentPlayer();
            }
        }



    }

    private void AddCurrentPlayerToBoard()
    {
        if (dice == null || currentPlayer == null)
            return;

        //Add player to board
        currentPlayer.currentTileIndex = 0;
        currentPlayer.moveToIndex = 0;

        currentPlayer.Scale(grid.TileScaleFactor);
        TileController t = grid.GetTileAtIndex(0);
        Vector2 startPos = t.transform.position;
        currentPlayer.JumpToCoord(startPos);
        

        if (currentPlayer.isMoveAllowed == false)
        {
            //reset dice turn
            dice.playerTurn = 0;
        }

        currentPlayer.isMoveAllowed = true;
    }


    void moveCurrentPlayer()
    {
        if (currentPlayer.currentTileIndex < 0)
        {
            return;
        }

        if ((currentPlayer.currentTileIndex == _tilesList.Count - 1))
        {
            GameOver(true);
        }

        var playerMoveToIndex = currentPlayer.currentTileIndex + dice.diceValue;
        //var playerMoveToIndex = currentPlayer.currentTileIndex + 1;



        if (playerMoveToIndex >= _tilesList.Count - 1)
        {
            //Set to Max grid entry
            playerMoveToIndex = _tilesList.Count - 1;

        }

        currentPlayer.moveToIndex = playerMoveToIndex;


        MovePlayer(currentPlayer.moveToIndex);


        if (!currentPlayer.isMoveAllowed)
        {
            dice.playerTurn = 0;
        }




    }

    void MovePlayer(int position, bool isMoveModified=false)
    {
        //Vector2 targetPosition = GetTilePosition(position);  // Get the target position based on the tile number
        if(currentPlayer.isMoveAllowed)
            StartCoroutine(currentPlayer.MoveToPosition(applyMoveModifier, isMoveModified));  // Move the player smoothly to the target position
    }

    void applyMoveModifier()
    {
        int modifier = GetMoveModifier(currentPlayer.currentTileIndex);
        if (modifier != 0)
        {
            currentPlayer.moveToIndex = currentPlayer.currentTileIndex + modifier;
            MovePlayer(modifier,true);
        }
    }

    // Convert the current position into a tile position in the world (use your own tile positions)
    Vector2 GetTilePosition(int tileNumber)
    {
        var t = grid.GetTileAtIndex(tileNumber);
        return t.transform.position;

    }

    /*

    private void MovePlayer(int moves=0)
    {

        if (player.currentTileIndex < 0)
        {
            return;
        }



        //get new position to move to
        //var playerMoveToIndex = player.currentTileIndex + dice.diceValue;
        //var playerMoveToIndex = player.currentTileIndex + 1;

        var playerMoveToIndex = player.currentTileIndex + moves;

        if (playerMoveToIndex >= _tilesList.Count - 1)
        {
            //Set to Max grid entry
            playerMoveToIndex = _tilesList.Count - 1;

        }


        if (player.isMoveAllowed == false)
        {
            //if player stopped moving, check if there is a move modifier on the current tile
            var moveModifier = GetMoveModifier(player.currentTileIndex);
            if (moveModifier != 0)
            {
                playerMoveToIndex += moveModifier;
            }
            else
            {
                //reset dice turn
                dice.playerTurn = 0;
            }

        }

        //var moveModifier = GetMoveModifier(player.currentTileIndex);
        //if (moveModifier != 0)
        //{
        //    playerMoveToIndex += moveModifier;

        //}
        //else
        //{
        //    isMoveModified = false;
        //}


        //set the new coordinates for the player
        player.moveToIndex = playerMoveToIndex;

        //tell the player to move
        player.isMoveAllowed = true;

        if (player.currentTileIndex == _tilesList.Count - 1)
        {
            GameOver(true);
        }




    }

    private void MoveModifier()
    {
        Debug.Log("MoveModifier " + isMoveModified + " - " + player.isMoveAllowed);

        if (isMoveModified)
        {
            var modifier = GetMoveModifier(player.currentTileIndex);
            MovePlayer(modifier);
        }





    }
    */

    /*
    private IEnumerator MoveModifer()
    {
        Debug.Log("InCoRoutine");
        if (player.isMoveAllowed == false)
        {
            var modifier = GetMoveModifier(player.currentTileIndex);
            player.moveToIndex = player.currentTileIndex + modifier;


            //tell the player to move if there was a modifier
            player.isMoveAllowed = true;


        }
        yield return new WaitForSeconds(0.2f);

    }
    */
    /*
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


    }*/


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
