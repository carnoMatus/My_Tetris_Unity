using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TetrominoSpawner
{
    private static readonly List<HashSet<(int, int)>> templates = new List<HashSet<(int, int)>>();
    private static Random random = new Random();

    static TetrominoSpawner()
    {
        templates.Add(new HashSet<(int, int)>() { (0,-1), (0,0), (0,1), (1,0) }); // T-Shaped
        templates.Add(new HashSet<(int, int)>() { (0,-1), (0,0), (0, 1), (0,2) }); // Straight
        templates.Add(new HashSet<(int, int)>() { (0,0), (0,1), (1,0), (1,1) }); // Box
        templates.Add(new HashSet<(int, int)>() { (-1,0), (0,0), (1,0), (1,1)} ); // L-Shaped
        templates.Add(new HashSet<(int, int)>() { (-1,0), (0,0), (1,0), (1,-1) }); // Reverse-L
        templates.Add(new HashSet<(int, int)>() { (0,1), (0,0), (1,0), (1,-1) }); // ZigZag
        templates.Add(new HashSet<(int, int)>() { (0,0), (0,1), (1,-1), (1,0) }); // Reverse-ZigZag
    }

    public static Tetromino GenerateTetromino()
    {
        int choice = random.Next(templates.Count);
        return new Tetromino(templates[choice], 0, 5, choice);
    }
}
