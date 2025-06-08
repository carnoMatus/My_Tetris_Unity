using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    

    private GameManager gm;

    private float dropTimer;

    void Start()
    {
        gm = GameManager.Instance;
        gm.SetGridManager(gridManager);
        gm.PrintSituation();
    }

    void Update()
    {
        if (!gm.Playing) return;

        HandleInput();

        dropTimer += Time.deltaTime;
        if (dropTimer >= gm.GetDropTime())
        {
            dropTimer = 0f;
            gm.MoveTetrominoDown();
        }
        gm.PrintSituation();
    }

    void HandleInput()
    {
        while (gm.FallDown)
        {
            gm.MoveTetrominoDown();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            gm.MoveTetrominoLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            gm.MoveTetrominoRight();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            gm.FallDown = true;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            gm.RotateTetromino();
    }
}
