using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public int currentTileIndex = -1;
    public int moveToIndex = -1;
    public string playerName = "Player";
    public float modifierMoveDelay = 0.15f;
    public bool isComputerPlayer = false;
    public bool isReady = false;

    public GridController grid;
    public bool isMoveAllowed = true;



    private SpriteRenderer spriteRenderer;
    private List<TileController> tileList;


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
    }

    private void Start()
    {
        tileList = grid._tilesList;
    }

    public void JumpToCoord(Vector2 coordinates)
    {
        transform.position = coordinates;
        isMoveAllowed = false;

    }



    // Smooth movement towards the target position
    public IEnumerator Move(System.Action checkMoveModifier, bool isModifiedMove=false)
    {
        
        isMoveAllowed = false;

        if (!isModifiedMove)
        {
            //Move to original target position
            while (currentTileIndex != moveToIndex)
            {
                currentTileIndex++;

                while (transform.position != tileList[currentTileIndex].transform.position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, tileList[currentTileIndex].transform.position, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }

        }
        else
        {
            yield return new WaitForSeconds(modifierMoveDelay);
            while (transform.position != tileList[moveToIndex].transform.position)
            {
                transform.position = Vector2.MoveTowards(transform.position, tileList[moveToIndex].transform.position, moveSpeed * Time.deltaTime);
                yield return null;

            }
        }

        currentTileIndex = moveToIndex;
       

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
