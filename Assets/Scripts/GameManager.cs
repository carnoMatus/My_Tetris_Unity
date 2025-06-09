using UnityEngine;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static readonly int GridHeight = 20;
    public static readonly int GridWidth = 10;
    private static readonly float Acceleration = 0.007f;
    private int score;
    private int highScore;
    private int level;
    private int totalRowsCleared;
    private string highScoreFilePath;
    private int[,] grid;
    private Tetromino tetromino;
    [SerializeField] private AudioSource clearRowAudio;

    public bool FallDown { get; set; }
    public GridManager GridManager { get; set; }
    [SerializeField] private float dropTime = 0.5f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameController gameController;

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

    private GameManager()
    {
        grid = new int[GridHeight, GridWidth];
    }

    public void PrintSituation()
    {
        if (SceneManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
        Tile[,] tiles = GridManager.GetTiles();
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                (int, int) relative = (i - tetromino.GetCenterPosition().Item1, j - tetromino.GetCenterPosition().Item2);
                Tile currentTile = tiles[GridHeight - 1 - i, j];
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
                    currentTile.Render(Tetromino.GetColorByIndex(grid[i, j] - 1), (i + j) % 2 == 0);
                    // again 0 cant be there
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
            return false; // still above the screen
        }
        return position.Item1 >= GridHeight ||
            position.Item2 >= GridWidth ||
            position.Item2 < 0 ||
            grid[position.Item1, position.Item2] != 0;
    }

    private void HandleFinishedRows()
    {
        int rowsCleared = 0;
        for (int i = 0; i < GridHeight; i++)
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
                rowsCleared++;
                for (int row = i; row > 0; row--)
                {
                    for (int col = 0; col < GridWidth; col++)
                    {
                        grid[row, col] = grid[row - 1, col];
                    }
                }
                for (int col = 0; col < GridWidth; col++)
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

    private void CementTetromino()
    {
        foreach ((int, int) position in tetromino.GetPositions())
        {
            if (position.Item1 + tetromino.GetCenterPosition().Item1 < 0) // we are above the screen
            {
                HandleScoreAndStop();
                return;
            }
            grid[position.Item1 + tetromino.GetCenterPosition().Item1, position.Item2 + tetromino.GetCenterPosition().Item2]
                = tetromino.GetColorIndex() + 1; // 1 because 0 is an color index but we don't want zeros
        }
        HandleFinishedRows();
        tetromino = TetrominoSpawner.GenerateTetromino();
        if (!CheckSpawnIsOK())
        {
            HandleScoreAndStop();
            return;
        }
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
        grid = new int[GridHeight, GridWidth];
        score = 0;
        level = 0;
        totalRowsCleared = 0;

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

    private void HandleScoreAndStop()
    {
        gameController.SetGameToNotPlaying("Press SPACE to restart...\n\nESC to quit", GameState.Restart);
        FallDown = false;
        SceneManager.Instance.GameState = GameState.Restart;
        if (highScore < score)
        {
            highScore = score;
        }
        Debug.Log("restarting");
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