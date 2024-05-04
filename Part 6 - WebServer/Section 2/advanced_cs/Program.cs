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
    }

    static List<Player> GetKnights(int level = 50)
    {
        List<Player> players = _players.Where(player => player.ClassType == ClassType.Knight && player.Level >= level).ToList();
        players.Sort((player1, player2) => player1.Level - player2.Level);
        return players;
    }

    static IOrderedEnumerable<Player> GetKnightsLinq(int level = 50)
    {
        return from p in _players
            where p.ClassType == ClassType.Knight && p.Level >= 50
            orderby p.Level
            select p;
    }
}