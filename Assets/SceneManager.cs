using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using UnityEngine;
using TMPro;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public GameState GameState { get; set; } = GameState.StartMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}