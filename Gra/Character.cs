namespace ClassesIntro;

public abstract class Character
{
    public const int MaxFatigue = 100;
    private const int ExhaustionDamage = 5;
    private const int FatiguePerMove = 5;

    protected Vector2 position;
    private readonly char _avatar;
    private readonly ConsoleColor _avatarColor;

    public Vector2 Position => position;
    public int MaxHealth { get; }
    public int Health { get; private set; }
    public int Fatigue { get; private set; }
    public bool IsAlive => Health > 0;

    protected Character(Vector2 startingPosition, char avatar, ConsoleColor avatarColor, int maxHealth)
    {
        position = startingPosition;
        _avatar = avatar;
        _avatarColor = avatarColor;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

    public void Display()
    {
        Console.SetCursorPosition(position.x, position.y);
        GameColors.WriteColored(_avatar, _avatarColor);
    }

    public void Move(Vector2 diff)
    {
        Move(diff.x, diff.y);
    }

    public void Move(int diffX, int diffY)
    {
        position = new Vector2(position.x + diffX, position.y + diffY);
        IncreaseFatigue(FatiguePerMove);
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    private void IncreaseFatigue(int amount)
    {
        Fatigue = Math.Min(MaxFatigue, Fatigue + amount);

        if (Fatigue >= MaxFatigue)
        {
            TakeDamage(ExhaustionDamage);
            Fatigue = 0;
        }
    }

    public abstract void ChooseAction(Game game);
}
