namespace advanced_cs;

public enum ClassType
{
    Knight,
    Archer,
    Mage,
    None
}

public class Player
{
    public ClassType ClassType { get; set; }
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public List<int>? Items { get; set; } = new List<int>();
}

class Program
{
    static List<Player> _players = new List<Player>();
    public static void Main(string[] args)
    {
        Random rand = new Random();
        for (int i = 0; i < 100; ++i)
        {
            ClassType type = rand.Next(0, 3) switch
            {
                0 => ClassType.Knight,
                1 => ClassType.Archer,
                2 => ClassType.Mage,
                _ => ClassType.None
            };

            Player player = new Player
            {
                ClassType = type,
                Level = rand.Next(1, 101),
                Hp = rand.Next(100, 1001),
                Attack = rand.Next(5, 51)
            };

            for (int j = 0; j < 5; ++j)
            {
                player.Items!.Add(rand.Next(1, 101));
            }
            
            _players.Add(player);
        }

        // Default
        {
            Console.WriteLine("Default");
            var players = GetKnights();
            foreach (var p in players)
            {
                Console.WriteLine($"{p.Level}, {p.Hp}");
            }
        }
        
        // Linq
        {
            Console.WriteLine("Linq");
            var players = GetKnightsLinq();
            foreach (var p in players)
            {
                Console.WriteLine($"{p.Level}, {p.Hp}");
            }
        }

        // nested from
        {
            var items = from player in _players
                from item in player.Items
                where item < 30
                select new { player, item };

            var itemList = items.ToList();
        }

        // grouping
        {
            var playersByLevel = from p in _players
                group p by p.Level
                into g
                orderby g.Key
                select new { g.Key, Players = g };
        }

        // join
        {
            List<int> levels = new List<int> { 2, 6, 10 };

            var playerLevels = from p in _players
                join l in levels
                    on p.Level equals l
                select p;
        }
        
        // std operator
        {
            var players = _players.Where(player => player is { ClassType: ClassType.Knight, Level: >= 50 })
                .OrderBy(player => player.Level).Select(player => player);
        }
    }

    static IEnumerable<Player> GetKnights(int level = 50)
    {
        return _players.Where(player => player is { ClassType: ClassType.Knight, Level: >= 50 })
            .OrderBy(player => player.Level).Select(player => player);
    }

    static IOrderedEnumerable<Player> GetKnightsLinq(int level = 50)
    {
        return from p in _players
            where p.ClassType == ClassType.Knight && p.Level >= 50
            orderby p.Level
            select p;
    }
}