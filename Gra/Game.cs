namespace ClassesIntro;

public class Game
{
    private const int EnemyCount = 2;
    private const int FriendlyCount = 1;
    private const int PotionCount = 3;
    private const int TreasureGoal = 3;

    private readonly Map _map = new();
    private readonly Player _player;
    private readonly List<Npc> _npcs = new();

    private bool _isGameOver;
    private bool _playerWon;
    private string _lastMessage = "";
    private ConsoleColor _lastMessageColor = GameColors.InfoText;

    public Game(string mapPath, Vector2 playerStartPosition)
    {
        _map.LoadMap(mapPath);
        _player = new Player(playerStartPosition);
        SetupWorld(playerStartPosition);
    }

    private void SetupWorld(Vector2 playerStartPosition)
    {
        HashSet<Vector2> occupied = new() { playerStartPosition };

        for (int i = 0; i < FriendlyCount; i++)
        {
            Vector2 pos = _map.GetRandomFreePosition(occupied);
            occupied.Add(pos);
            _npcs.Add(new Npc(pos, NpcKind.Friendly, "Podróżniku, uważaj na wrogów w okolicy!"));
        }

        for (int i = 0; i < EnemyCount; i++)
        {
            Vector2 pos = _map.GetRandomFreePosition(occupied);
            occupied.Add(pos);
            _npcs.Add(new Npc(pos, NpcKind.Hostile));
        }

        for (int i = 0; i < PotionCount; i++)
        {
            Vector2 pos = _map.GetRandomFreePosition(occupied);
            occupied.Add(pos);
            _map.GetCell(pos).ItemOnCell = Item.CreateHealingPotion();
        }

        for (int i = 0; i < TreasureGoal; i++)
        {
            Vector2 pos = _map.GetRandomFreePosition(occupied);
            occupied.Add(pos);
            _map.GetCell(pos).ItemOnCell = Item.CreateTreasure();
        }
    }

    public void Run()
    {
        Console.CursorVisible = false;
        RedrawAll();

        while (!_isGameOver)
        {
            _player.ChooseAction(this);

            if (!_isGameOver)
            {
                foreach (Npc npc in _npcs.ToList())
                {
                    if (_isGameOver)
                    {
                        break;
                    }

                    npc.ChooseAction(this);
                }
            }

            if (!_isGameOver)
            {
                RedrawAll();
            }
        }

        ShowEndScreen();
    }

    public Character? GetCharacterAt(Vector2 position, Character exclude)
    {
        if (!ReferenceEquals(_player, exclude) && _player.Position.Equals(position))
        {
            return _player;
        }

        foreach (Npc npc in _npcs)
        {
            if (!ReferenceEquals(npc, exclude) && npc.Position.Equals(position))
            {
                return npc;
            }
        }

        return null;
    }

    public void TryMovePlayer(Player player, int diffX, int diffY)
    {
        Vector2 target = player.Position + new Vector2(diffX, diffY);

        Character? blocker = GetCharacterAt(target, player);
        if (blocker is Npc npc)
        {
            ResolveInteraction(npc);
            return;
        }

        if (!_map.IsWalkable(target))
        {
            SetMessage("Droga jest zablokowana przez ścianę.", GameColors.WarningText);
            return;
        }

        player.Move(diffX, diffY);
        TryPickUpItem(player, target);

        if (!player.IsAlive)
        {
            EndGame(won: false, "Wyczerpanie i rany Cię pokonały...");
        }
    }

    public void TryMoveNpc(Npc npc, int diffX, int diffY)
    {
        Vector2 target = npc.Position + new Vector2(diffX, diffY);

        if (!_map.IsWalkable(target))
        {
            return;
        }

        if (GetCharacterAt(target, npc) != null)
        {
            return;
        }

        npc.Move(diffX, diffY);
    }

    private void TryPickUpItem(Player player, Vector2 position)
    {
        Cell cell = _map.GetCell(position);
        Item? item = cell.ItemOnCell;

        if (item == null)
        {
            return;
        }

        cell.ItemOnCell = null;
        player.Inventory.Add(item);
        SetMessage($"Znaleziono: {item.Name}", GameColors.HealText);

        if (item.Type == ItemType.Treasure)
        {
            player.AddTreasure();

            if (player.TreasureCollected >= TreasureGoal)
            {
                EndGame(won: true, "Zebrano wszystkie skarby! Wygrywasz!");
            }
        }
    }

    private void ResolveInteraction(Npc npc)
    {
        if (npc.Kind == NpcKind.Friendly)
        {
            ShowFriendlyDialogue(npc);
        }
        else
        {
            ResolveCombat(npc);
        }
    }

    private void ShowFriendlyDialogue(Npc npc)
    {
        Console.Clear();
        GameColors.WriteLineColored("=== ROZMOWA ===", GameColors.FriendlyNpc);
        Console.WriteLine(npc.DialogueText);

        if (npc.TryTakeGift(out bool alreadyTaken))
        {
            Item gift = Item.CreateHealingPotion();
            _player.Inventory.Add(gift);
            Console.WriteLine($"Podróżnik daje Ci przedmiot: {gift.Name}!");
        }
        else if (alreadyTaken)
        {
            Console.WriteLine("Nie mam nic więcej dla Ciebie.");
        }

        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do gry...");
        Console.ReadKey(true);
    }

    private void ResolveCombat(Npc enemy)
    {
        while (_player.IsAlive && enemy.IsAlive)
        {
            Console.Clear();
            GameColors.WriteLineColored("=== WALKA ===", GameColors.HostileNpc);
            Console.WriteLine($"Twoje zdrowie: {_player.Health}/{_player.MaxHealth}");
            Console.WriteLine($"Zdrowie wroga: {enemy.Health}/{enemy.MaxHealth}");
            Console.WriteLine();
            Console.WriteLine("[ENTER] Atakuj    [Q] Uciekaj");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Q)
            {
                Console.WriteLine("Uciekasz z walki!");
                Console.ReadKey(true);
                return;
            }

            int playerDamage = Random.Shared.Next(8, 16);
            enemy.TakeDamage(playerDamage);
            GameColors.WriteLineColored($"Trafiasz wroga za {playerDamage} obrażeń.", GameColors.HealText);

            if (!enemy.IsAlive)
            {
                Console.WriteLine("Wróg pokonany!");
                _npcs.Remove(enemy);
                Console.ReadKey(true);
                break;
            }

            int enemyDamage = Random.Shared.Next(enemy.AttackPower, enemy.AttackPower + 6);
            _player.TakeDamage(enemyDamage);
            GameColors.WriteLineColored($"Wróg trafia Cię za {enemyDamage} obrażeń.", GameColors.DamageText);
            Console.ReadKey(true);
        }

        if (!_player.IsAlive)
        {
            EndGame(won: false, "Zginąłeś w walce...");
        }
    }

    public void ShowInventory(Player player)
    {
        Console.Clear();
        player.Inventory.Display();
        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do gry...");
        Console.ReadKey(true);
    }

    public void UseItemMenu(Player player)
    {
        Console.Clear();

        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("Twój ekwipunek jest pusty.");
            Console.ReadKey(true);
            return;
        }

        player.Inventory.Display();
        Console.WriteLine("Wybierz numer przedmiotu do użycia (lub inny klawisz, aby wrócić):");

        ConsoleKeyInfo key = Console.ReadKey(true);
        Item? item = player.Inventory.GetItemByIndex(key.KeyChar - '1');

        if (item == null)
        {
            return;
        }

        if (item.Type == ItemType.Potion)
        {
            player.Heal(item.Value);
            player.Inventory.Remove(item);
            GameColors.WriteLineColored($"Używasz {item.Name}. Odzyskujesz {item.Value} zdrowia.", GameColors.HealText);
        }
        else
        {
            Console.WriteLine($"{item.Name} nie może zostać teraz użyty.");
        }

        Console.ReadKey(true);
    }

    public void DropItemMenu(Player player)
    {
        Console.Clear();

        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("Nie masz nic do wyrzucenia.");
            Console.ReadKey(true);
            return;
        }

        player.Inventory.Display();
        Console.WriteLine("Wybierz numer przedmiotu do wyrzucenia (lub inny klawisz, aby wrócić):");

        ConsoleKeyInfo key = Console.ReadKey(true);
        Item? item = player.Inventory.GetItemByIndex(key.KeyChar - '1');

        if (item == null)
        {
            return;
        }

        player.Inventory.Remove(item);
        _map.GetCell(player.Position).ItemOnCell = item;
        Console.WriteLine($"Wyrzucasz {item.Name} na ziemię.");
        Console.ReadKey(true);
    }

    public void QuitGame()
    {
        EndGame(won: false, "Zakończono grę.");
    }

    private void SetMessage(string text, ConsoleColor color)
    {
        _lastMessage = text;
        _lastMessageColor = color;
    }

    private void EndGame(bool won, string message)
    {
        _isGameOver = true;
        _playerWon = won;
        _lastMessage = message;
    }

    private void RedrawAll()
    {
        Console.Clear();
        _map.Display();

        foreach (Npc npc in _npcs)
        {
            npc.Display();
        }

        _player.Display();

        DrawHud();
    }

    private void DrawHud()
    {
        Console.SetCursorPosition(0, _map.Height);
        GameColors.WriteLineColored(new string('-', _map.Width), GameColors.HudText);

        GameColors.WriteLineColored(
            $"HP: {_player.Health}/{_player.MaxHealth}  Zmęczenie: {_player.Fatigue}/{Character.MaxFatigue}  " +
            $"Skarby: {_player.TreasureCollected}/{TreasureGoal}  Ekwipunek: {_player.Inventory.Count}",
            GameColors.HudText);

        Console.WriteLine("Strzałki - ruch | I - ekwipunek | U - użyj | R - wyrzuć | Q - wyjście");

        if (!string.IsNullOrEmpty(_lastMessage))
        {
            GameColors.WriteLineColored(_lastMessage, _lastMessageColor);
        }
    }

    private void ShowEndScreen()
    {
        Console.Clear();
        ConsoleColor headerColor = _playerWon ? GameColors.HealText : GameColors.DamageText;
        GameColors.WriteLineColored(_playerWon ? "=== WYGRAŁEŚ! ===" : "=== KONIEC GRY ===", headerColor);
        Console.WriteLine(_lastMessage);
        Console.CursorVisible = true;
    }
}
