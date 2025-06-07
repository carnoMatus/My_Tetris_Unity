using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Tetromino
{
    private static readonly Color[] COLORS = { Color.green, Color.deepPink,
        Color.darkOrange, Color.red, Color.blue, Color.cyan, Color.magenta };
    private HashSet<(int, int)> relativePositions;
    private (int, int) centerPosition;

    private int colorIndex;

    public Tetromino(System.Collections.Generic.HashSet<(int, int)> positions, int y, int x, int colorIndex)
    {
        this.centerPosition = (y, x);
        this.relativePositions = positions;
        this.colorIndex = colorIndex;
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

    public bool MoveDown()
    {
        foreach (var (y, x) in relativePositions)
        {
            if (GameManager.Instance.TileClashes((y + centerPosition.Item1 + 1, x + centerPosition.Item2)))
            {
                return false;
            }
        }
        centerPosition.Item1++;
        return true;
    }

    private bool MoveToSide(int offset)
    {
        foreach (var (y, x) in relativePositions)
        {
            if (GameManager.Instance.TileClashes((y + centerPosition.Item1, x + centerPosition.Item2 + offset)))
            {
                return false;
            }
        }
        centerPosition.Item2 += offset;
        return true;
    }

    public bool MoveRight()
    {
        return MoveToSide(1);
    }

    public bool MoveLeft()
    {
        return MoveToSide(-1);
    }

    public bool Rotate()
    {
        var newPositions = new HashSet<(int, int)>();

        foreach (var (y, x) in relativePositions)
        {
            newPositions.Add((-x, y));
            if (GameManager.Instance.TileClashes((-x + centerPosition.Item1, y + centerPosition.Item2)))
            {
                return false;
            }
        }
        relativePositions = newPositions;
        return true;
    }
}
