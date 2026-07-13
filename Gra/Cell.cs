namespace ClassesIntro;

public class Cell
{
    private readonly char _terrainSymbol;

    public Item? ItemOnCell { get; set; }

    public bool IsWalkable => _terrainSymbol != '#';

    public Cell(char terrainSymbol)
    {
        _terrainSymbol = terrainSymbol;
    }

    public void Display()
    {
        if (ItemOnCell != null)
        {
            GameColors.WriteColored(ItemOnCell.Symbol, ItemOnCell.Color);
            return;
        }

        ConsoleColor color = _terrainSymbol == '#' ? GameColors.Wall : GameColors.Floor;
        GameColors.WriteColored(_terrainSymbol, color);
    }
}
