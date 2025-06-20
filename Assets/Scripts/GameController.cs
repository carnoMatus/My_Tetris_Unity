using System;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private AudioSource downKeyAudio;
    private GameManager gm;
    private SceneManager sm;
    private float dropTimer;

    void Start()
    {

    }

    void Update()
    {
        if (sm.GameState == GameState.Playing)
        {
            PlayingUpdate();
        }
    }

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        sm = gameManager.SceneManager;
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
            sm.PauseGame();
        }
    }
}