namespace ClassesIntro;

public class Map
{
    private readonly List<List<Cell>> _cells = new();

    public int Height => _cells.Count;
    public int Width => _cells.Count > 0 ? _cells[0].Count : 0;

    public void LoadMap(string path)
    {
        _cells.Clear();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            List<Cell> row = new();

            foreach (char symbol in line)
            {
                row.Add(new Cell(symbol));
            }

            _cells.Add(row);
        }
    }

    public void Display()
    {
        foreach (List<Cell> row in _cells)
        {
            foreach (Cell cell in row)
            {
                cell.Display();
            }

            Console.WriteLine();
        }
    }

    public Cell GetCell(Vector2 position)
    {
        return _cells[position.y][position.x];
    }

    public bool IsInsideBounds(Vector2 position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    public bool IsWalkable(Vector2 position)
    {
        return IsInsideBounds(position) && GetCell(position).IsWalkable;
    }

    public Vector2 GetRandomFreePosition(HashSet<Vector2> occupied)
    {
        while (true)
        {
            Vector2 candidate = new Vector2(Random.Shared.Next(Width), Random.Shared.Next(Height));

            if (IsWalkable(candidate) && !occupied.Contains(candidate))
            {
                return candidate;
            }
        }
    }
}
