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
    private int currentPlayerIndex = 0;
    private GameObject[] players;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _tilesList = grid._tilesList;
        players = GameObject.FindGameObjectsWithTag("Player"); //Find all players
        currentPlayer = players[currentPlayerIndex].GetComponent<PlayerController>();
    }

    private void Update()
    {

        dice.transform.position = new Vector2(grid.transform.localScale.magnitude + 5, grid.transform.localScale.y / 2.0f);

  

        if (dice.isDiceRolled && currentPlayer.isMoveAllowed)
        {
            if (dice.diceValue == 6 && currentPlayer.currentTileIndex < 0)
                AddCurrentPlayerToBoard();
            else
            {
                MoveCurrentPlayer();
            }
            dice.isDiceRolled = false;
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
        

        currentPlayer.isMoveAllowed = true;
    }


    void MoveCurrentPlayer()
    {
        if (currentPlayer.currentTileIndex < 0)
        {
            return;
        }


        var playerMoveToIndex= currentPlayer.currentTileIndex + dice.diceValue;
        //var playerMoveToIndex = currentPlayer.currentTileIndex + 1; // can use for testing to move 1 space at a time



        if (playerMoveToIndex >= _tilesList.Count - 1)
        {
            //Set to Max grid entry
            playerMoveToIndex = _tilesList.Count - 1;

        }

        currentPlayer.moveToIndex = playerMoveToIndex;

        if (currentPlayer.isMoveAllowed && dice.isDiceRolled)
        {
            MovePlayer(currentPlayer.moveToIndex);
        }


    }

    //Starts the coroutine for movement
    void MovePlayer(int position, bool isMoveModified=false)
    {
        if(currentPlayer.isMoveAllowed)
            StartCoroutine(currentPlayer.Move(EndMove, isMoveModified));  // Move the player smoothly to the target position
    }

    /// <summary>
    /// Gets the movement modifier (dragon or vine) of the current player's current tile
    /// and tells the player to move according to the modifier
    /// </summary>
    bool ApplyMoveModifier()
    {
        
        int modifier = GetMoveModifier(currentPlayer.currentTileIndex);
        if (modifier != 0)
        {
            Debug.Log("Modifier applied: " + modifier);
            currentPlayer.moveToIndex = currentPlayer.currentTileIndex + modifier;
            MovePlayer(modifier,true);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Checks for any movements that still need to be applied, 
    /// then checks if end of the board is reached and then switches player
    /// i.e. 
    ///     1. Checks if the player is on a dragon/vine and moves the player
    ///     2. Check if the player has won the game
    ///     3. Goes to the next player's turn
    /// 
    /// </summary>
    void EndMove()
    {
        bool isMoveModified = ApplyMoveModifier();
        if ((currentPlayer.currentTileIndex == _tilesList.Count - 1))
        {
            GameOver(true);
        }

        if (!isMoveModified)
        {
            NextTurn();
        }

    }

    void NextTurn()
    {
        
        if (dice.diceValue != 6)
        {
            Debug.Log("Turn from player " + currentPlayerIndex + " to " + (currentPlayerIndex + 1) % players.Length);
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            currentPlayer = players[currentPlayerIndex].GetComponent<PlayerController>(); ;
            dice.playerTurn = currentPlayerIndex+1;
        }
     

    }

    // Convert the current position into a tile position in the world (Not currently used - Keep for now)
    Vector2 GetTilePosition(int tileNumber)
    {
        var t = grid.GetTileAtIndex(tileNumber);
        return t.transform.position;

    }


    public void GameOver(bool hasWon)
    {
        //Win game
        string winMsg = currentPlayer.playerName + " wins!";
        GameUI.instance.GameOver(hasWon, winMsg);
        this.isGameOver = true;
        Time.timeScale = 0.0f;
        //Show menu etc.
    }

    /// <summary>
    /// Helper method to get the movement modifier of a tile and a specified index
    /// </summary>
    /// <param name="TileIndex">Tile index to get the modifier from</param>
    /// <returns></returns>
    private int GetMoveModifier(int TileIndex) 
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
