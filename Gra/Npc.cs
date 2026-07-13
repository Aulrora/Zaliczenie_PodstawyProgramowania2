namespace ClassesIntro;

public enum NpcKind
{
    Friendly,
    Hostile
}

public class Npc : Character
{
    private static readonly Vector2[] AvailableDirections =
    {
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(0, 1)
    };

    private bool _hasGivenGift;

    public NpcKind Kind { get; }
    public string DialogueText { get; }
    public int AttackPower { get; }

    public Npc(Vector2 startingPosition, NpcKind kind, string dialogueText = "", int maxHealth = 40, int attackPower = 8)
        : base(
            startingPosition,
            kind == NpcKind.Hostile ? 'E' : 'V',
            kind == NpcKind.Hostile ? GameColors.HostileNpc : GameColors.FriendlyNpc,
            maxHealth)
    {
        Kind = kind;
        DialogueText = dialogueText;
        AttackPower = attackPower;
    }

    public bool TryTakeGift(out bool alreadyTaken)
    {
        alreadyTaken = _hasGivenGift;

        if (_hasGivenGift)
        {
            return false;
        }

        _hasGivenGift = true;
        return true;
    }

    public override void ChooseAction(Game game)
    {
        Vector2 direction = AvailableDirections[Random.Shared.Next(AvailableDirections.Length)];
        game.TryMoveNpc(this, direction.x, direction.y);
    }
}
