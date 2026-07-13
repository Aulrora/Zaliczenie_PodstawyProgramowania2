namespace ClassesIntro;

public static class GameColors
{
    // teren mapy 
    public static ConsoleColor Wall = ConsoleColor.DarkGray;
    public static ConsoleColor Floor = ConsoleColor.Gray;

    // postacie
    public static ConsoleColor Player = ConsoleColor.Cyan;
    public static ConsoleColor HostileNpc = ConsoleColor.Red;
    public static ConsoleColor FriendlyNpc = ConsoleColor.Green;

    // przedmioty
    public static ConsoleColor Potion = ConsoleColor.Magenta;
    public static ConsoleColor Treasure = ConsoleColor.Yellow;

    // teksty / komunikaty
    public static ConsoleColor HudText = ConsoleColor.White;
    public static ConsoleColor InfoText = ConsoleColor.Yellow;
    public static ConsoleColor WarningText = ConsoleColor.DarkRed;
    public static ConsoleColor DamageText = ConsoleColor.Red;
    public static ConsoleColor HealText = ConsoleColor.Green;

    public static void WriteColored(char symbol, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(symbol);
        Console.ResetColor();
    }

    public static void WriteLineColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
