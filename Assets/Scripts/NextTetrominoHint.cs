using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NextTetrominoHint : MonoBehaviour
{
    public Image preview;

    public Sprite TShaped;
    public Sprite Straight;
    public Sprite Box;
    public Sprite LShaped;
    public Sprite ReverseL;
    public Sprite Zigzag;
    public Sprite ReverseZigzag;

    private Sprite[] sprites;
    
    void Start()
    {
        preview.enabled = false;
        sprites = new Sprite[]{ TShaped, Straight, Box, LShaped, ReverseL, Zigzag, ReverseZigzag };
    }

    public void ChangeTexture(int index)
    {
        preview.sprite = sprites[index];        
    }
}
