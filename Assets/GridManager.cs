using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform camera;

    private Tile[,] tileGrid = new Tile[20, 10];


    void Start()
    {
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tileGrid[y, x] = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                tileGrid[y, x].name = $"Tile {x},{y}";

                bool offset = (x + y) % 2 != 0;
                tileGrid[y, x].Render(offset);
            }
        }
        camera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public Tile[,] GetTiles()
    {
        return tileGrid;
    }
}
