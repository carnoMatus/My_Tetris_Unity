using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public bool FallDown { get; set; }
    private static readonly float Acceleration = 0.007f;
    private int score;
    private int highScore;
    private int level;
    private int totalRowsCleared;
    private string highScoreFilePath;
    private int[,] grid;
    private Tetromino tetromino;
    private Tetromino tetrominoNext;
    private int gridWidth, gridHeight;
    private float dropTime = 0.5f;

    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private AudioSource clearRowAudio;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text nextText;
    [SerializeField] private SceneManager sceneManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameController gameController;
    [SerializeField] private NextTetrominoHint nextTetrominoHint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        gridWidth = gameSettings.gridWidth;
        gridHeight = gameSettings.gridHeight;
        dropTime = gameSettings.dropTime;

        Instance = this;
        gridManager.Initialize(this);
        gameController.Initialize(this);

        highScoreFilePath = Path.Combine(Application.persistentDataPath, "highscore.txt");
        grid = new int[gridHeight, gridWidth];
        DontDestroyOnLoad(gameObject);
    }

    public SceneManager SceneManager => sceneManager;
    public GridManager GridManager => gridManager;
    public GameController GameController => gameController;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    public void RefreshGameScreen()
    {
        if (sceneManager.GameState != GameState.Playing)
        {
            return;
        }
        Tile[,] tiles = GridManager.GetTiles();
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Vector2Int relative = new Vector2Int(x - tetromino.GetCenterPosition().x, y - tetromino.GetCenterPosition().y);
                Tile currentTile = tiles[gridHeight - 1 - y, x];
                if (tetromino.GetPositions().Contains(relative))
                {
                    currentTile.Render(tetromino.GetColor(), (y + x) % 2 == 0);
                }
                else if (grid[y, x] == 0)
                {
                    currentTile.Render((y + x) % 2 == 0);
                }
                else
                {
                    currentTile.Render(Tetromino.GetColorByIndex(grid[y, x] - 1), (y + x) % 2 == 0);
                    // again 0 cant be there
                }
            }
        }
        highScoreText.text = "HIGH SCORE: " + highScore;
        if (sceneManager.GameState != GameState.Restart)
        {
            scoreText.text = "SCORE: " + score;
            return;
        }
        scoreText.text = "FINAL SCORE: " + score;
    }

    public void RefreshTetromino(Action move)
    {
        HashSet<Vector2Int> tetrominoBefore = tetromino.GetTotalPositions();
        move?.Invoke();
        HashSet<Vector2Int> tetrominoAfter = tetromino.GetTotalPositions();
        Tile[,] tiles = GridManager.GetTiles();
        tetrominoBefore.ExceptWith(tetrominoAfter);

        foreach (var position in tetrominoBefore)
        {
            if (position.y < 0)
            {
                continue; // still above the screen
            }
            Tile currentTile = tiles[gridHeight - 1 - position.y, position.x];
            if (grid[position.y, position.x] == 0)
            {
                currentTile.Render((position.y + position.x) % 2 == 0);
            }
        }
        foreach (var position in tetrominoAfter)
        {
            if (position.y < 0)
            {
                continue; // still above the screen
            }
            Tile currentTile = tiles[gridHeight - 1 - position.y, position.x];
            currentTile.Render(tetromino.GetColor(), (position.y + position.x) % 2 == 0);
        }
    }

    public bool TileClashes(Vector2Int position)
    {
        if (position.y < 0)
        {
            return false; // still above the screen
        }
        return position.y >= gridHeight ||
            position.x >= gridWidth ||
            position.x < 0 ||
            grid[position.y, position.x] != 0;
    }

    private void HandleFinishedRows()
    {
        int rowsCleared = 0;
        EffectTile[,] effectTiles = gridManager.GetEffectTiles();
        for (int i = 0; i < gridHeight; i++)
        {
            bool full = true;
            for (int j = 0; j < GridWidth; j++)
            {
                if (grid[i, j] <= 0)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int k = 0; k < gridWidth; k++)
                {
                    effectTiles[gridHeight - i - 1, k].StartFadeOut();
                }
                rowsCleared++;
                for (int row = i; row > 0; row--)
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        grid[row, col] = grid[row - 1, col];
                    }
                }
                for (int col = 0; col < gridWidth; col++)
                {
                    grid[0, col] = 0;
                }
                i--;
            }
        }
        HandleScore(rowsCleared);
        if (rowsCleared > 0)
        {
            clearRowAudio?.Play();
        }
        dropTime = 0.8f - Acceleration * level;
    }

    public void CementTetromino()
    {
        foreach (var position in tetromino.GetPositions())
        {
            if (position.y + tetromino.GetCenterPosition().y < 0) // we are above the screen
            {
                HandleScoreAndStop();
                return;
            }
            grid[position.y + tetromino.GetCenterPosition().y, position.x + tetromino.GetCenterPosition().x]
                = tetromino.GetColorIndex() + 1; // 1 because 0 is an color index but we don't want zeros
        }
        HandleFinishedRows();
        tetromino = tetrominoNext;
        tetrominoNext = TetrominoSpawner.GenerateTetromino(gridWidth);
        nextTetrominoHint.ChangeTexture(tetrominoNext.GetColorIndex());
        if (!CheckSpawnIsOK())
        {
            HandleScoreAndStop();
            return;
        }
        FallDown = false;
        RefreshGameScreen();
    }

    public float GetDropTime()
    {
        return dropTime;
    }

    public void MoveTetrominoDown()
    {
        RefreshTetromino(tetromino.MoveDown);
    }

    public void MoveTetrominoRight()
    {
        RefreshTetromino(tetromino.MoveRight);
    }

    public void MoveTetrominoLeft()
    {
        RefreshTetromino(tetromino.MoveLeft);
    }

    public void RotateTetromino()
    {
        RefreshTetromino(tetromino.Rotate);
    }

    public void StartNew()
    {
        nextText.text = "NEXT:";
        nextTetrominoHint.preview.enabled = true;
        grid = new int[gridHeight, gridWidth];
        score = 0;
        level = 0;
        totalRowsCleared = 0;

        tetrominoNext = TetrominoSpawner.GenerateTetromino(gridWidth);
        nextTetrominoHint.ChangeTexture(tetrominoNext.GetColorIndex());
        tetromino = TetrominoSpawner.GenerateTetromino(gridWidth);
        FallDown = false;
        RefreshGameScreen();
    }

    private bool CheckSpawnIsOK()
    {
        foreach (var position in tetromino.GetTotalPositions())
        {
            if (TileClashes(position))
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
    }

    private void HandleScoreAndStop()
    {
        gameController.SetGameToNotPlaying("Press SPACE to restart...\n\nESC to quit", GameState.Restart);
        FallDown = false;
        sceneManager.GameState = GameState.Restart;
        if (highScore < score)
        {
            highScore = score;
            StoreHighScore();
        }
    }

    private void HandleScore(int rowsCleared)
    {
        int gain;
        switch (rowsCleared)
        {
            case 0:
                gain = 0;
                break;
            case 1:
                gain = 40 * (level + 1);
                break;
            case 2:
                gain = 100 * (level + 1);
                break;
            case 3:
                gain = 300 * (level + 1);
                break;
            default:
                gain = 1200 * (level + 1);
                break;
        }
        score += gain;
        totalRowsCleared += rowsCleared;
        level = totalRowsCleared / 10;
    }
}