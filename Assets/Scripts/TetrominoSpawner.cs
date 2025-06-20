using System.Collections.Generic;
using UnityEngine;


public static class TetrominoSpawner
{
    private static readonly List<HashSet<Vector2Int>> templates = new List<HashSet<Vector2Int>>();

    static TetrominoSpawner()
    {
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(-1,0),
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1)
        }); // T-Shaped
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(-1,0),
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0)
        }); // Straight
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1)
        }); // Box
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(0,-1),
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1)}
        ); // L-Shaped
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(0,-1),
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(-1,1)
        }); // Reverse-L
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(1,0),
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(-1,1)
        }); // ZigZag
        templates.Add(new HashSet<Vector2Int>() {
            new Vector2Int(0,0),
            new Vector2Int(-1,0),
            new Vector2Int(1,1),
            new Vector2Int(0,1)
        }); // Reverse-ZigZag
    }

    public static Tetromino GenerateTetromino(int gridWidth)
    {
        int choice = UnityEngine.Random.Range(0, templates.Count);
        return new Tetromino(GameManager.Instance, templates[choice], new Vector2Int(gridWidth / 2, 0), choice);
    }
}
