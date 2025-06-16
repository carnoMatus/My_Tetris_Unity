using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;


public class Tetromino
{
    private static readonly Color[] COLORS = { Color.green, Color.deepPink,
        Color.darkOrange, Color.red, Color.blue, Color.cyan, Color.magenta };
    private HashSet<(int, int)> relativePositions;
    private (int, int) centerPosition;

    private GameManager gameManager;

    private int colorIndex;

    public Tetromino(GameManager gm, System.Collections.Generic.HashSet<(int, int)> positions, int y, int x, int colorIndex)
    {
        centerPosition = (y, x);
        relativePositions = positions;
        this.colorIndex = colorIndex;
        gameManager = gm;
    }

    public int GetColorIndex()
    {
        return colorIndex;
    }
    public Color GetColor()
    {
        return COLORS[colorIndex];
    }

    public static Color GetColorByIndex(int index)
    {
        return COLORS[index];
    }

    public HashSet<(int, int)> GetPositions()
    {
        return relativePositions;
    }

    public (int, int) GetCenterPosition()
    {
        return centerPosition;
    }

    public HashSet<(int, int)> GetTotalPositions()
    {
        var totalPositions = new HashSet<(int, int)>();
        foreach (var relativePosition in relativePositions)
        {
            totalPositions.Add((centerPosition.Item1 + relativePosition.Item1, centerPosition.Item2 + relativePosition.Item2));
        }
        return totalPositions;
    }

    public void MoveDown()
    {
        foreach (var (y, x) in relativePositions)
        {
            if (gameManager.TileClashes((y + centerPosition.Item1 + 1, x + centerPosition.Item2)))
            {
                gameManager.CementTetromino();
                return;
            }
        }
        centerPosition.Item1++;
    }

    private void MoveToSide(int offset)
    {
        foreach (var (y, x) in relativePositions)
        {
            if (gameManager.TileClashes((y + centerPosition.Item1, x + centerPosition.Item2 + offset)))
            {
                return;
            }
        }
        centerPosition.Item2 += offset;
    }

    public void MoveRight()
    {
        MoveToSide(1);
    }

    public void MoveLeft()
    {
        MoveToSide(-1);
    }

    public void Rotate()
    {
        bool succeeded = false;
        HashSet<(int, int)> newPositions = new HashSet<(int, int)>();
        while (!succeeded)
        {
            succeeded = true;
            newPositions.Clear();

            foreach (var (y, x) in relativePositions)
            {
                newPositions.Add((x, -y));
                if (gameManager.TileClashes((-x + centerPosition.Item1, y + centerPosition.Item2)))
                {
                    succeeded = false;
                    if (centerPosition.Item2 < gameManager.GridWidth / 2)
                    {
                        centerPosition.Item2++;
                    }
                    else
                    {
                        centerPosition.Item2--;
                    }
                    break;
                }
            }
        }
        
        relativePositions = newPositions;
    }
}
