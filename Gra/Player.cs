namespace ClassesIntro;

public class Player : Character
{
    public Inventory Inventory { get; } = new();
    public int TreasureCollected { get; private set; }

    public Player(Vector2 startingPosition) : base(startingPosition, '@', GameColors.Player, maxHealth: 100)
    {
    }

    public void AddTreasure()
    {
        TreasureCollected++;
    }

    public override void ChooseAction(Game game)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                game.TryMovePlayer(this, 0, -1);
                break;
            case ConsoleKey.DownArrow:
                game.TryMovePlayer(this, 0, 1);
                break;
            case ConsoleKey.LeftArrow:
                game.TryMovePlayer(this, -1, 0);
                break;
            case ConsoleKey.RightArrow:
                game.TryMovePlayer(this, 1, 0);
                break;
            case ConsoleKey.I:
                game.ShowInventory(this);
                break;
            case ConsoleKey.U:
                game.UseItemMenu(this);
                break;
            case ConsoleKey.R:
                game.DropItemMenu(this);
                break;
            case ConsoleKey.Q:
                game.QuitGame();
                break;
        }
    }
}
