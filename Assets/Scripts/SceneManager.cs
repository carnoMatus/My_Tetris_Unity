using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameState GameState { get; set; } = GameState.StartMenu;
    private GameManager gm;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitToMenuButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private AudioSource soundtrack;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioLowPassFilter soundtrackLowPass;
    [SerializeField] private GameObject sizeButtonGroup;
    [SerializeField] private GameSettings classicSize;
    [SerializeField] private GameObject tetrisLogo;


    void Start()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        quitToMenuButton.gameObject.SetActive(false);
        quitGameButton.gameObject.SetActive(true);
        tetrisLogo.gameObject.SetActive(true);

        soundtrack.loop = true;
        soundtrack?.Play();
        ApplyPauseAudioEffect(true);

    }

    public void GameEnded()
    {
        GameState = GameState.Restart;
        restartButton.gameObject.SetActive(true);
        quitToMenuButton.gameObject.SetActive(true);

        ApplyPauseAudioEffect(true);
    }

    public void StartNewGame()
    {
        GameState = GameState.Playing;
        gm.StartNew();
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        quitToMenuButton.gameObject.SetActive(false);
        quitGameButton.gameObject.SetActive(false);
        tetrisLogo.gameObject.SetActive(false);

        sizeButtonGroup.SetActive(false);

        ApplyPauseAudioEffect(false);
        clickAudio?.Play();
    }

    public void PauseGame()
    {
        GameState = GameState.Paused;
        resumeButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        quitToMenuButton.gameObject.SetActive(true);

        ApplyPauseAudioEffect(true);
        clickAudio?.Play();
    }

    public void ResumeGame()
    {
        GameState = GameState.Playing;
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        quitToMenuButton.gameObject.SetActive(false);

        ApplyPauseAudioEffect(false);
        clickAudio?.Play();
    }

    public void QuitToMenu()
    {
        GameState = GameState.StartMenu;
        gm.GridManager.Clear();
        gm.SetSize(classicSize);

        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        quitToMenuButton.gameObject.SetActive(false);
        sizeButtonGroup.SetActive(true);
        quitGameButton.gameObject.SetActive(true);
        tetrisLogo.gameObject.SetActive(true);

        gm.HideUIElements();
    }


    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
    }

    private void ApplyPauseAudioEffect(bool paused)
    {
        soundtrackLowPass.enabled = paused;
        soundtrackLowPass.cutoffFrequency = paused ? 1000f : 22000f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}