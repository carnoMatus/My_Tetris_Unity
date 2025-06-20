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
    [SerializeField] private Button pauseButton;
    [SerializeField] private AudioSource soundtrack;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioLowPassFilter soundtrackLowPass;


    void Start()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        soundtrack.loop = true;
        soundtrack?.Play();
        ApplyPauseAudioEffect(true);

    }

    public void GameEnded()
    {
        GameState = GameState.Restart;
        restartButton.gameObject.SetActive(true);

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

        ApplyPauseAudioEffect(false);
        clickAudio?.Play();
    }

    public void PauseGame()
    {
        GameState = GameState.Paused;
        resumeButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);

        ApplyPauseAudioEffect(true);
        clickAudio?.Play();
    }

    public void ResumeGame()
    {
        GameState = GameState.Playing;
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);

        ApplyPauseAudioEffect(false);
        clickAudio?.Play();
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
}