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

        dice.transform.position = new Vector2(grid.transform.localScale.magnitude + 6, grid.transform.localScale.y / 2.0f);



        if (GameUI.instance.message != null)
        {
            GameUI.instance.GeneralMessage("Current Player Turn: " + currentPlayer.name);
        }

        //TODO: figure out the computer player dice roll
        if (currentPlayer.isComputerPlayer)
        {
            AutoRoll();

        }

        if (!dice.isDiceRolled)
        {
            GameUI.instance.GeneralMessage(currentPlayer.name + " please roll the dice");
            return;
        }


        if (!currentPlayer.GetSelectedBead())
        {
            GameUI.instance.GeneralMessage(currentPlayer.name + " please select a bead");
            return;
        }

        if (dice.isDiceRolled && currentPlayer.isMoveAllowed)
        {
            if (dice.diceValue > 0)
                currentPlayer.AddDiceRoll(dice.diceValue);

            if (dice.diceValue == 6 && !currentPlayer.IsReady())
            {
                AddCurrentPlayerToBoard();
            }

            else if (currentPlayer.IsReady())
            {
                MoveCurrentPlayer();
            }
            else
            {
                NextTurn();
            }
            dice.isDiceRolled = false;
            currentPlayer.AddDiceRollHistory();

        }


    }

    private void AutoRoll()
    {
        //Step 1: Roll the Dice
        /* Conditions:
         *  - Must be the Computer Player's turn
         *  - No other player must be moving
         *  - Current Player's move must be allowed
         *  - Dice must not be rolled
         */
        if (currentPlayer.isMoveAllowed && !dice.isDiceRolled)
        {
            dice.Roll();
        }

        //Step 2: Select a bead
        /* Conditions:
         *  - Dice must be rolled
         *  - No other bead is selected
         */
        //if (dice.isDiceRolled)
        //{
        //    currentPlayer.GetSelectedBead();
        //}

        //-> Player bead add to board must be called if correct conditions
        //-> Player bead must move if correct conditions
        //-> Player turn ends


    }
    private void AddCurrentPlayerToBoard()
    {
        if (dice == null || currentPlayer == null)
            return;

        //Add player to board
        //currentPlayer.currentTileIndex = -1;
        //currentPlayer.moveToIndex = -1;
        currentPlayer.Scale(grid.TileScaleFactor);
        currentPlayer.JumpToCoord(grid.ReadyArea);
        currentPlayer.isMoveAllowed = true;
        currentPlayer.selectedBead.isReady = true;


    }


    void MoveCurrentPlayer()
    {

        if (!currentPlayer.selectedBead.isReady)
            return;


        //var playerMoveToIndex= currentPlayer.selectedBead.currentTileIndex + dice.diceValue;
        var playerMoveToIndex = currentPlayer.selectedBead.currentTileIndex + currentPlayer.GetCurrentDiceRoll();
        //var playerMoveToIndex = currentPlayer.currentTileIndex + 1; // can use for testing to move 1 space at a time


        if (playerMoveToIndex >= _tilesList.Count - 1)
        {
            //Set to Max grid entry
            playerMoveToIndex = _tilesList.Count - 1;

        }

        //currentPlayer.moveToIndex = playerMoveToIndex;

        //Additional check to see if co-routine is running before moving
        if (currentPlayer.isMoveAllowed && dice.isDiceRolled)
        {
            MovePlayer(dice.diceValue);
        }


    }

    //Starts the coroutine for movement
    void MovePlayer(int moves, bool isMoveModified=false)
    {
        if(currentPlayer.isMoveAllowed)
            StartCoroutine(currentPlayer.Move(moves, EndMove, isMoveModified));  // Move the player smoothly to the target position
    }

    /// <summary>
    /// Gets the movement modifier (dragon or vine) of the current player's current tile
    /// and tells the player to move according to the modifier
    /// </summary>
    bool ApplyMoveModifier()
    {
        
        int modifier = GetMoveModifier(currentPlayer.selectedBead.currentTileIndex);
        if (modifier != 0)
        {
            Debug.Log("Modifier applied: " + modifier);
            //currentPlayer.moveToIndex = currentPlayer.currentTileIndex + modifier;
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
        if ((currentPlayer.selectedBead.currentTileIndex == _tilesList.Count - 1))
        {
            GameOver(true);
        }

        if (!isMoveModified)
        {
            if (dice.diceValue != 6)
            {
                NextTurn();
            }
        }

    }


    void NextTurn()
    {

        Debug.Log("Turn from player " + currentPlayerIndex + " to " + (currentPlayerIndex + 1) % players.Length);
        currentPlayer.DeselectCurrentBead();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        currentPlayer = players[currentPlayerIndex].GetComponent<PlayerController>();
        //dice.playerTurn = currentPlayerIndex + 1;
        dice.isDiceRolled = false;


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
        
        foreach (var player in players)
        {
            var playerController = player.GetComponent<PlayerController>();
            var playerRollHistory = playerController.GetHistoryDiceRoll();
            string rollHistory = "Player- " + playerController.playerName + ": ";

            foreach (var roll in playerRollHistory)
            {
                    rollHistory += roll + ",";
            }
            rollHistory = rollHistory.Substring(0, rollHistory.Length - 1);
            Debug.Log(rollHistory);
        }
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
