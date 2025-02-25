using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    public Color baseColour, offsetColour;
    public GameObject highlight;


    public int TileNum;
    public int moveModifier = 0;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }


    public void Init(bool isOffset)
    {
        spriteRenderer.color = isOffset ? baseColour : offsetColour;
 
    }

    public void Scale(Vector2 scaleFactor)
    {
        spriteRenderer.transform.localScale = scaleFactor;
        highlight.transform.localScale = scaleFactor;

    }
}
