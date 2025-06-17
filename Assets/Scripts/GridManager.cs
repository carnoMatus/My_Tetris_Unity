using UnityEngine;

public class GridManager : MonoBehaviour
{
    private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private EffectTile effectTilePrefab;
    [SerializeField] private new Transform camera;
    private Tile[,] tileGrid;
    private EffectTile[,] effectGrid;
    void Start()
    {

    }

    public void GenerateGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tileGrid[y, x] = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                tileGrid[y, x].name = $"Tile {x},{y}";
                effectGrid[y, x] = Instantiate(effectTilePrefab, new Vector3(x, y), Quaternion.identity);
                effectGrid[y, x].name = $"EffectTile {x},{y}";

                bool offset = (x + y) % 2 != 0;
                tileGrid[y, x].Render(offset);
            }
        }
        camera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public void Initialize(GameManager gameManager)
    {
        width = gameManager.GridWidth;
        height = gameManager.GridHeight;
        tileGrid = new Tile[height, width];
        effectGrid = new EffectTile[height, width];
        GenerateGrid();
    }

    public Tile[,] GetTiles()
    {
        return tileGrid;
    }

    public EffectTile[,] GetEffectTiles()
    {
        return effectGrid;
    }
}
