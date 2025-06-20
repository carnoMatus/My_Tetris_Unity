using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NextTetrominoHint : MonoBehaviour
{
    public Image preview;

    [SerializeField] private Sprite TShaped;
    [SerializeField] private Sprite Straight;
    [SerializeField] private Sprite Box;
    [SerializeField] private Sprite LShaped;
    [SerializeField] private Sprite ReverseL;
    [SerializeField] private Sprite Zigzag;
    [SerializeField] private Sprite ReverseZigzag;

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
