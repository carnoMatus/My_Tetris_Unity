using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static readonly int GRID_HEIGHT = 20;
    public static readonly int GRID_WIDTH = 10;
    private double tick = 500;
    public bool Playing { get; private set; } = true;
    private int score;
    private int[,] grid;
    private Tetromino tetromino;
    public bool FallDown { get; set; }

    public TMP_Text scoreText;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Optional, if you want only one GameManager
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional, if it should persist between scenes
    }

    public GridManager GridManager { get; private set; }

    public void SetGridManager(GridManager gridManager)
    {
        GridManager = gridManager;
    }


    private GameManager()
    {
        grid = new int[GRID_HEIGHT, GRID_WIDTH];
        score = 0;
        tetromino = TetrominoSpawner.GenerateTetromino();
        FallDown = false;
    }

    public void PrintSituation()
    {
        Tile[,] tiles = GridManager.GetTiles();
        for (int i = 0; i < GRID_HEIGHT; i++)
        {
            for (int j = 0; j < GRID_WIDTH; j++)
            {
                (int, int) relative = (i - tetromino.GetCenterPosition().Item1, j - tetromino.GetCenterPosition().Item2);
                Tile currentTile = tiles[GRID_HEIGHT - 1 - i, j];
                if (tetromino.GetPositions().Contains(relative))
                {
                    currentTile.Render(tetromino.GetColor(), (i + j) % 2 == 0);
                }
                else if (grid[i, j] == 0)
                {
                    currentTile.Render((i + j) % 2 == 0);
                }
                else
                {
                    currentTile.Render(Tetromino.GetColorByIndex(grid[i, j] - 1), (i + j) % 2 == 0); // again 0 cant be there
                }
            }
        }
        if (Playing)
        {
            scoreText.text = "SCORE: " + score;
            Debug.Log("did it bruh\n");
            return;
        }
        scoreText.text = "FINAL SCORE: " + score;
    }

    public bool TileClashes((int, int) position)
    {
        if (position.Item1 < 0)
        {
            return false; //still above the screen
        }
        return (position.Item1 >= GRID_HEIGHT ||
            position.Item2 >= GRID_WIDTH ||
            position.Item2 < 0 ||
            grid[position.Item1, position.Item2] != 0);
    }

    private void HandleFinishedRows()
    {
        int scoreToAdd = 0;
        for (int i = 0; i < GRID_HEIGHT; i++)
        {
            bool full = true;
            for (int j = 0; j < GRID_WIDTH; j++)
            {
                if (grid[i, j] <= 0)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                scoreToAdd++;
                for (int row = i; row > 0; row--)
                {
                    for (int col = 0; col < GRID_WIDTH; col++)
                    {
                        grid[row, col] = grid[row - 1, col];
                    }
                }
                for (int col = 0; col < GRID_WIDTH; col++)
                {
                    grid[0, col] = 0;
                }
                i--;
            }
        }
        score += (5000 * scoreToAdd) / (int)tick;
    }

    private void CementTetromino()
    {
        foreach ((int, int) position in tetromino.GetPositions())
        {
            if (position.Item1 + tetromino.GetCenterPosition().Item1 < 0) //we are above screen
            {
                FallDown = false;
                Playing = false;
                PrintSituation();
                return;
            }
            grid[position.Item1 + tetromino.GetCenterPosition().Item1, position.Item2 + tetromino.GetCenterPosition().Item2]
                = tetromino.GetColorIndex() + 1; //1 cause 0 is an index but we dont want zeros
        }
        HandleFinishedRows();
        tetromino = TetrominoSpawner.GenerateTetromino();
        tick *= 0.99;
        FallDown = false;
    }

    public void MoveTetrominoDown()
    {
        if (!tetromino.MoveDown())
        {
            CementTetromino();
        }
    }

    public void MoveTetrominoRight()
    {
        tetromino.MoveRight();
    }

    public void MoveTetrominoLeft()
    {
        tetromino.MoveLeft();
    }

    public void RotateTetromino()
    {
        tetromino.Rotate();
    }
}