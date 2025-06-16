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
    public GameState GameState { get; set; } = GameState.StartMenu;
}