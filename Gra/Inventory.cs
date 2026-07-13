namespace ClassesIntro;

public class Inventory
{
    private readonly List<Item> _items = new();

    public int Count => _items.Count;

    public void Add(Item item)
    {
        _items.Add(item);
    }

    public bool Remove(Item item)
    {
        return _items.Remove(item);
    }

    public Item? GetItemByIndex(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            return null;
        }

        return _items[index];
    }

    public void Display()
    {
        Console.WriteLine("=== EKWIPUNEK ===");

        if (_items.Count == 0)
        {
            Console.WriteLine("(pusto)");
        }
        else
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Item item = _items[i];
                GameColors.WriteLineColored($"{i + 1}. {item.Name} [{item.Symbol}]", item.Color);
            }
        }

        Console.WriteLine("=================");
    }
}
