using System;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private AudioSource downKeyAudio;
    [SerializeField] private AudioSource soundtrack;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioLowPassFilter soundtrackLowPass;
    private GameManager gm;
    private SceneManager sm;
    private float dropTimer;

    void Start()
    {
        soundtrack.loop = true;
        soundtrack?.Play();
        ApplyPauseAudioEffect(true);
        gm.LoadHighScore();
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

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        sm = gameManager.SceneManager;
    }

    private void MenuUpdate()
    {
        messageText.SetText("Press SPACE to start...\n\nESC to quit");
        messageText.gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameToPlaying(true);
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
        {
            gm.MoveTetrominoLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gm.MoveTetrominoRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gm.MoveTetrominoDown();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            downKeyAudio?.Play();
            gm.FallDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gm.RotateTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetGameToNotPlaying("Press SPACE to resume...\n\nR to restart\n\nESC to quit", GameState.Paused);
        }
    }

    private void PausedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameToPlaying(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SafeExit();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetGameToPlaying(true);
        }
    }

    private void RestartUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameToPlaying(true);
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

    private void ApplyPauseAudioEffect(bool paused)
    {
        soundtrackLowPass.enabled = paused;
        soundtrackLowPass.cutoffFrequency = paused ? 1200f : 22000f;
    }

    private void SetGameToPlaying(bool restart)
    {
        ApplyPauseAudioEffect(false);
        sm.GameState = GameState.Playing;
        messageText.gameObject.SetActive(false);
        clickAudio?.Play();
        if (restart)
        {
            gm.StartNew();
        }
    }

    public void SetGameToNotPlaying(String message, GameState state)
    {
        ApplyPauseAudioEffect(true);
        sm.GameState = state;
        messageText.SetText(message);
        messageText.gameObject.SetActive(true);
        clickAudio?.Play();
    }
}
