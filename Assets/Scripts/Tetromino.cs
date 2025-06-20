using System.Collections.Generic;
using UnityEngine;


public class Tetromino
{
    private static readonly Color[] COLORS = { Color.green, Color.deepPink,
        Color.darkOrange, Color.red, Color.blue, Color.cyan, Color.magenta };
    private HashSet<Vector2Int> relativePositions;
    private Vector2Int centerPosition;

    private GameManager gameManager;

    private int colorIndex;

    public Tetromino(GameManager gm, System.Collections.Generic.HashSet<Vector2Int> positions, Vector2Int position, int colorIndex)
    {
        centerPosition = position;
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

    public HashSet<Vector2Int> GetPositions()
    {
        return relativePositions;
    }

    public Vector2Int GetCenterPosition()
    {
        return centerPosition;
    }

    public HashSet<Vector2Int> GetTotalPositions()
    {
        var totalPositions = new HashSet<Vector2Int>();
        foreach (var relativePosition in relativePositions)
        {
            totalPositions.Add(new Vector2Int(centerPosition.x + relativePosition.x, centerPosition.y + relativePosition.y));
        }
        return totalPositions;
    }

    public void MoveDown()
    {
        foreach (var position in relativePositions)
        {
            if (gameManager.TileClashes(new Vector2Int(position.x + centerPosition.x, position.y + centerPosition.y + 1)))
            {
                gameManager.CementTetromino();
                return;
            }
        }
        centerPosition.y++;
    }

    private void MoveToSide(int offset)
    {
        foreach (var position in relativePositions)
        {
            if (gameManager.TileClashes(new Vector2Int(position.x + centerPosition.x + offset, position.y + centerPosition.y)))
            {
                return;
            }
        }
        centerPosition.x += offset;
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
        bool succeeded = true;
        int offset = 0;
        HashSet<Vector2Int> newPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            succeeded = true;
            newPositions.Clear();

            foreach (var position in relativePositions)
            {
                Vector2Int newPosition = new Vector2Int(-position.y, position.x);
                newPositions.Add(newPosition);
                if ((newPosition + centerPosition).x < 0)
                {
                    succeeded = false;
                    centerPosition.x++;
                    offset++;
                    break;
                }
                else if ((newPosition + centerPosition).x >= gameManager.GridWidth)
                {
                    succeeded = false;
                    centerPosition.x--;
                    offset--;
                    break;
                }
                else if (gameManager.TileClashes(newPosition + centerPosition))
                {
                    succeeded = false;
                }
            }
            if (succeeded)
            {
                break;
            }
        }
        if (succeeded) {
            relativePositions = newPositions;
        } else
        {
            centerPosition.x -= offset;
        }
    }
}
