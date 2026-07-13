namespace ClassesIntro;

public enum ItemType
{
    Potion,
    Treasure
}

public class Item
{
    public string Name { get; }
    public char Symbol { get; }
    public ItemType Type { get; }
    public int Value { get; }
    public ConsoleColor Color { get; }

    public Item(string name, char symbol, ItemType type, int value, ConsoleColor color)
    {
        Name = name;
        Symbol = symbol;
        Type = type;
        Value = value;
        Color = color;
    }

    public static Item CreateHealingPotion()
    {
        return new Item("Mikstura leczenia", '!', ItemType.Potion, 30, GameColors.Potion);
    }

    public static Item CreateTreasure()
    {
        return new Item("Starożytna moneta", '$', ItemType.Treasure, 1, GameColors.Treasure);
    }
}
