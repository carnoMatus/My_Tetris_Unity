using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static readonly int GRID_HEIGHT = 20;
    public static readonly int GRID_WIDTH = 10;
    private double tick = 500;
    private int score;
    private int highScore;
    private string highScoreFilePath;
    [SerializeField] private AudioSource clearRowAudio;
    private int[,] grid;
    private Tetromino tetromino;
    public bool FallDown { get; set; }

    [SerializeField] private float dropTime = 0.5f;

    public TMP_Text scoreText;

    public TMP_Text highScoreText;

    public static GameManager Instance { get; private set; }

    private static readonly float ACCELARATION = 0.99f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        highScoreFilePath = Path.Combine(Application.persistentDataPath, "highscore.txt");
        DontDestroyOnLoad(gameObject);
    }
    public GridManager GridManager { get; private set; }
    public void SetGridManager(GridManager gridManager)
    {
        GridManager = gridManager;
    }

    private GameManager()
    {
        grid = new int[GRID_HEIGHT, GRID_WIDTH];
    }

    public void PrintSituation()
    {
        if (SceneManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
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
        highScoreText.text = "HIGH SCORE: " + highScore;
        if (SceneManager.Instance.GameState != GameState.Restart)
        {
            scoreText.text = "SCORE: " + score;
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
        if (scoreToAdd > 0)
        {
            clearRowAudio?.Play();
        }
    }

    private void CementTetromino()
    {
        foreach ((int, int) position in tetromino.GetPositions())
        {
            if (position.Item1 + tetromino.GetCenterPosition().Item1 < 0) //we are above screen
            {
                FallDown = false;
                SceneManager.Instance.GameState = GameState.Restart;
                if (highScore < score)
                {
                    highScore = score;
                }
                return;
            }
            grid[position.Item1 + tetromino.GetCenterPosition().Item1, position.Item2 + tetromino.GetCenterPosition().Item2]
                = tetromino.GetColorIndex() + 1; //1 cause 0 is an index but we dont want zeros
        }
        HandleFinishedRows();
        tetromino = TetrominoSpawner.GenerateTetromino();
        if (!CheckSpawnIsOK())
        {
            SceneManager.Instance.GameState = GameState.Restart;
            return;
        }

        tick *= 0.99;
        dropTime *= ACCELARATION;
        FallDown = false;
    }

    public float GetDropTime()
    {
        return dropTime;
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

    public void StartNew()
    {
        grid = new int[GRID_HEIGHT, GRID_WIDTH];
        score = 0;
        tetromino = TetrominoSpawner.GenerateTetromino();
        FallDown = false;
    }

    private bool CheckSpawnIsOK()
    {
        foreach (var (y, x) in tetromino.GetPositions())
        {
            (int, int) center = tetromino.GetCenterPosition();
            if (TileClashes((y + center.Item1, x + center.Item2)))
            {
                return false;
            }
        }
        return true;
    }

    public void LoadHighScore()
    {
        if (!File.Exists(highScoreFilePath))
        {
            File.WriteAllText(highScoreFilePath, "0");
        }

        string content = File.ReadAllText(highScoreFilePath);
        int.TryParse(content, out highScore);
    }

    public void StoreHighScore()
    {
        File.WriteAllText(highScoreFilePath, highScore.ToString());
        Debug.Log("Stored score " + highScore);
    }
}