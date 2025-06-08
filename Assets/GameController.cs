using System;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GridManager gridManager;


    private GameManager gm;
    private SceneManager sm;

    private float dropTimer;

    void Start()
    {
        gm = GameManager.Instance;
        sm = SceneManager.Instance;
        gm.SetGridManager(gridManager);
        gm.LoadHighScore();
        messageText.SetText("Press SPACE to start...\nESC to quit");
        messageText.gameObject.SetActive(true);
    }

    void Update()
    {
        switch (sm.GameState)
        {
            case GameState.StartMenu:
                MenuUpdate();
                break;
            case GameState.Playing:
                PlayingUpdate();
                break;
            case GameState.Paused:
                PausedUpdate();
                break;
            case GameState.Restart:
                RestartUpdate();
                break;
            default:
                break;
        }
    }

    private void MenuUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sm.GameState = GameState.Playing;
            gm.StartNew();
            messageText.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SafeExit();
        }
    }

    private void PlayingUpdate()
    {
        HandleInput();

        dropTimer += Time.deltaTime;
        if (dropTimer >= gm.GetDropTime())
        {
            dropTimer = 0f;
            gm.MoveTetrominoDown();
        }
        gm.PrintSituation();
    }

    void HandleInput()
    {
        while (gm.FallDown)
        {
            gm.MoveTetrominoDown();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            gm.MoveTetrominoLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            gm.MoveTetrominoRight();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            gm.FallDown = true;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            gm.RotateTetromino();
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            sm.GameState = GameState.Paused;
            messageText.SetText("Press SPACE to resume...\nESC to quit");
            messageText.gameObject.SetActive(true);
        }
    }

    private void PausedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sm.GameState = GameState.Playing;
            messageText.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SafeExit();
        }
    }

    private void RestartUpdate()
    {
        messageText.SetText("Press SPACE to restart...\nESC to quit");
        messageText.gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sm.GameState = GameState.Playing;
            messageText.gameObject.SetActive(false);
            gm.StartNew();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SafeExit();
        }
    }

    private void SafeExit()
    {
        gm.StoreHighScore();
        Application.Quit();
    }
}
