using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public int currentTileIndex = -1;
    public int moveToIndex = -1; 

    public GridManager grid;
    public bool isMoveAllowed = false;



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

    private void Update()
    {
        if (isMoveAllowed && currentTileIndex >=0)
        {
            Move();
        }

    }

    public void Move(Vector2 coordinates)
    {
        transform.position = coordinates;
        isMoveAllowed = false;

    }

    public void Move()
    {
        
        if (currentTileIndex <= tileList.Count - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, tileList[moveToIndex].transform.position, moveSpeed * Time.deltaTime);

            if (transform.position == tileList[moveToIndex].transform.position)
            {
                currentTileIndex = moveToIndex;
                moveToIndex = -1;
                isMoveAllowed = false;
            }
        }

    }

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
